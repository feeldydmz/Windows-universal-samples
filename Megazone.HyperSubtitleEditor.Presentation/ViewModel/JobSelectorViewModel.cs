using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Enum;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Reference;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class JobSelectorViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ILogger _logger;
        private readonly PresetLoader _presetLoader;
        private readonly ITranscodingRepository _transcodingRepository;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isConnectVideoChecked;
        private IList<JobItemViewModel> _jobs;
        private ICommand _loadedCommand;

        private ICommand _okCommand;

        private JobListPagingItemViewModel _pagingItemViewModel;
        private PagingViewModel _pagingViewModel;
        private ICommand _refreshCommand;
        private JobItemViewModel _selectedItem;
        private bool _shouldImportAllSubtitles = true;
        private ICommand _unloadedCommand;

        public JobSelectorViewModel(IBrowser browser,
            ITranscodingRepository transcodingRepository,
            ILogger logger,
            PresetLoader presetLoader)
        {
            _browser = browser;
            _transcodingRepository = transcodingRepository;
            _logger = logger;
            _presetLoader = presetLoader;
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand UnloadedCommand
        {
            get { return _unloadedCommand = _unloadedCommand ?? new RelayCommand(OnUnloaded); }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand = _refreshCommand ?? new RelayCommand(Refresh); }
        }

        public JobItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public IList<JobItemViewModel> Jobs
        {
            get => _jobs;
            private set => Set(ref _jobs, value);
        }

        public PagingViewModel PagingViewModel
        {
            get => _pagingViewModel;
            set => Set(ref _pagingViewModel, value);
        }

        public JobListPagingItemViewModel PagingItemViewModel
        {
            get => _pagingItemViewModel;
            set => Set(ref _pagingItemViewModel, value);
        }

        public ICommand OkCommand
        {
            get { return _okCommand = _okCommand ?? new RelayCommand(OnOk, CanOk); }
        }

        public bool ShouldImportAllSubtitles
        {
            get => _shouldImportAllSubtitles;
            set => Set(ref _shouldImportAllSubtitles, value);
        }

        public bool IsConnectVideoChecked
        {
            get => _isConnectVideoChecked;
            set => Set(ref _isConnectVideoChecked, value);
        }

        public int SelectedOutputIndex { get; set; }

        private bool CanOk()
        {
            return SelectedItem != null;
        }

        private void OnOk()
        {
            var mediaIndex = -1;
            if (IsConnectVideoChecked)
                mediaIndex = SelectedOutputIndex;
            MessageCenter.Instance.Send(new JobFoundMessage(this, SelectedItem.Data, mediaIndex,
                _shouldImportAllSubtitles));
        }

        private void OnLoaded()
        {
            PagingViewModel = new PagingViewModel(OnNextPageRequested, OnPreviousPageRequested);
            PagingItemViewModel = new JobListPagingItemViewModel();
            GetJobList(JobListPagingItemViewModel.PageIndexType.Current);
        }

        private void Refresh()
        {
            GetJobList(JobListPagingItemViewModel.PageIndexType.Current);
        }

        private void OnUnloaded()
        {
        }

        private Task FindAllJobs(JobListPagingItemViewModel pagingItem,
            JobListPagingItemViewModel.PageIndexType pageIndexType)
        {
            return this.CreateTask(() =>
            {
                FindAll(pagingItem,
                    pageIndexType,
                    (responsePagingItem, findAllJobsResult) =>
                    {
                        this.InvokeOnUi(() =>
                        {
                            if (pageIndexType == JobListPagingItemViewModel.PageIndexType.Next)
                            {
                                responsePagingItem.LastViewedPageIndex++;
                            }
                            else if (pageIndexType == JobListPagingItemViewModel.PageIndexType.Previous)
                            {
                                responsePagingItem.LastViewedPageIndex--;
                                if (responsePagingItem.LastViewedPageIndex < 0)
                                    responsePagingItem.LastViewedPageIndex = 0;
                            }

                            PagingViewModel.IsPagingEnabled = findAllJobsResult != null &&
                                                              findAllJobsResult.NextPageToken.IsNotNullOrAny() ||
                                                              responsePagingItem.ContinuationParameterCount > 0;
                            PagingViewModel.HasNext = findAllJobsResult?.NextPageToken.IsNotNullOrAny() ?? false;
                            PagingViewModel.HasPrevious = responsePagingItem.LastViewedPageIndex > 0;
                            if (!string.IsNullOrEmpty(findAllJobsResult?.NextPageToken))
                                responsePagingItem.SetContinuationParameter(findAllJobsResult.NextPageToken);

                            // TODO: loading
                            //_browser.Main.Transcoder.OuputGroup.JobList?.LoadingManager.Hide();
                        });
                    });
            });
        }

        private void FindAll(JobListPagingItemViewModel pagingItem,
            JobListPagingItemViewModel.PageIndexType pageIndexType,
            Action<JobListPagingItemViewModel, FindAllJobsResult> completedAction)
        {
            var findAllResult = FindAll(pagingItem, pageIndexType);
            if (findAllResult == null)
            {
                completedAction?.Invoke(pagingItem, null);
            }
            else
            {
                var jobitems = findAllResult.Items;
                _presetLoader.Load(new WeakAction<IList<Preset>>(presets =>
                    {
                        this.InvokeOnUi(() =>
                        {
                            AddToJobListItems(jobitems, presets);
                            completedAction?.Invoke(pagingItem, findAllResult);
                        });
                    }),
                    true);
            }
        }

        private FindAllJobsResult FindAll(JobListPagingItemViewModel pagingItem,
            JobListPagingItemViewModel.PageIndexType pageIndexType)
        {
            var parameter =
                new ParameterBuilder(AppContext.CredentialInfo).WithFindJobsParameter(new FindJobParameter
                    {
                        PipelineId = AppContext.Config.PipelineId,
                        NextPageToken = pagingItem.GetContinuationParameter(pageIndexType)
                    })
                    .Build();

            return _transcodingRepository.FindAllJobs(parameter);
        }

        private void OnNextPageRequested()
        {
            GetJobList(JobListPagingItemViewModel.PageIndexType.Next);
        }

        private void OnPreviousPageRequested()
        {
            GetJobList(JobListPagingItemViewModel.PageIndexType.Previous);
        }

        private async void GetJobList(JobListPagingItemViewModel.PageIndexType pageIndexType)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Jobs = new ObservableCollection<JobItemViewModel>();
            //var loadingManager = _browser.Main.Transcoder.OuputGroup.JobList.LoadingManager;
            try
            {
                //loadingManager.Show();
                await FindAllJobs(PagingItemViewModel, pageIndexType);
                //MessageCenter.Instance.Send(new JobList.RequestScrollToTopMessage(this));
            }
            catch (TaskCanceledException)
            {
                _logger.Debug.Write("refresh task cancel!!!");
                //loadingManager.Hide();
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.Debug.Write(e.Message);
                //loadingManager.Hide();
                _browser.ShowConfirmWindow(
                    new ConfirmWindowParameter(Resource.CNT_WARNING,
                        Resource.MSG_CONFIRM_ELASTIC_TRANSCODER_AUTHORITY,
                        MessageBoxButton.OK)); // TODO: 다국어
            }
        }

        private async void AddToJobListItems(IEnumerable<ITranscodingJob> jobItems,
            IList<Preset> presets)
        {
            Jobs.Clear();

            if (!_cancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            await this.CreateTask(() =>
            {
                foreach (var job in jobItems.Cast<Job>()
                    .ToList())
                {
                    if (job.Status != Status.Complete) continue;
                    var jobListItemViewModel = new JobItemViewModel(_browser, job)
                    {
                        InputVideoName = job.Inputs[0]
                            ?.Key,
                        InputOtherVideoCount = job.Inputs.Count - 1,
                        FinishTime = job.Timing?.FinishTimeMillis?.EpocToDateTime() ?? DateTime.MinValue,
                        Status = job.Status,
                        OutputKeyPrefix = job.OutputKeyPrefix
                    };
                    if (job.Inputs[0]
                            ?.DetectedProperties !=
                        null)
                    {
                        jobListItemViewModel.InputVideoResolution = job.Inputs[0]
                                                                        ?.DetectedProperties?.Width +
                                                                    " x " +
                                                                    job.Inputs[0]
                                                                        ?.DetectedProperties?.Height;
                        jobListItemViewModel.InputVideoSize = job.Inputs[0]
                            ?.DetectedProperties?.FileSize;
                        jobListItemViewModel.InputVideoDuration = job.Inputs[0]
                            ?.DetectedProperties?.DurationMillis;
                    }

                    // Playlist에서 Format과 상대 경로를 저장.
                    if (job.Playlists != null)
                        foreach (var playlist in job.Playlists)
                        {
                            var jobOutput = new JobListItemOutputViewModel
                            {
                                DisplayMediaType = MediaType.AdaptiveStreaming,
                                OutputKeys = playlist.OutputKeys,
                                DisplayName = playlist.Format.ToDisplayValue(),
                                RelativePath = playlist.Name,
                                OutputKeyPrefix = job.OutputKeyPrefix,
                                Extension = playlist.Format.GetExtension(),
                                OutputStatus = playlist.Status
                            };
                            jobOutput.Initialize();
                            jobListItemViewModel.Outputs.Add(jobOutput);
                        }

                    // job의 output를 기준으로 playlist의 outputkey와 매칭하여 presetId를 저장.
                    if (job.Outputs != null)
                        foreach (var output in job.Outputs)
                        {
                            var existOutputKey = false;

                            foreach (var itemOutput in jobListItemViewModel.Outputs)
                                if (itemOutput.OutputKeys.Contains(output.Key))
                                {
                                    itemOutput.PresetIds.Add(output.PresetId);
                                    existOutputKey = true;
                                }

                            // job의 output에는 있으나, playlist에 없는 output key의 경우 [(ex) video(mp4), audio]
                            if (!existOutputKey)
                            {
                                var extension = Path.GetExtension(output.Key);
                                string name;
                                if (string.IsNullOrEmpty(extension))
                                {
                                    var matchedPresetId = presets.FirstOrDefault(p => p.Id == output.PresetId);

                                    extension = matchedPresetId?.Container.ToString()
                                                    .ToLower() ??
                                                string.Empty;

                                    name = extension.ToUpper()
                                        .Trim();
                                }
                                else
                                {
                                    name = extension.Substring(1)
                                        .ToUpper()
                                        .Trim();
                                }
                                var jobOutput = new JobListItemOutputViewModel
                                {
                                    DisplayName = name,
                                    DisplayMediaType = name.ToMediaType(),
                                    RelativePath = output.Key,
                                    OutputKeyPrefix = job.OutputKeyPrefix,
                                    Extension = extension,
                                    OutputStatus = output.Status
                                };
                                jobOutput.Initialize();
                                jobOutput.OutputKeys.Add(output.Key);
                                jobOutput.PresetIds.Add(output.PresetId);

                                jobListItemViewModel.Outputs.Add(jobOutput);
                            }
                        }
                    jobListItemViewModel.Url = jobListItemViewModel.Outputs?.FirstOrDefault()
                        ?.FullUrl;
                    Thread.Sleep(1);
                    this.InvokeOnUi(() => { Jobs.Add(jobListItemViewModel); }, true);
                }
                this.InvokeOnUi(() => { new MediaDataLoader(Jobs, _cancellationTokenSource).Run(); });
            });
        }
    }
}