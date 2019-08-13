using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Api.Transcoder.Domain;
using Megazone.Api.Transcoder.Service;
using Megazone.Api.Transcoder.ServiceInterface;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
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
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data;
using Job = Megazone.Api.Transcoder.Domain.Job;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class JobSelectorViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly IJobService _jobService;
        private readonly ILogger _logger;
        private readonly PresetLoader _presetLoader;

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
            IJobService jobService,
            ILogger logger,
            PresetLoader presetLoader)
        {
            _browser = browser;
            _jobService = jobService;
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

                            PagingViewModel.IsPagingEnabled = findAllJobsResult?.LastEvaluatedKey != null ||
                                                              responsePagingItem.ContinuationParameterCount > 0;
                            PagingViewModel.HasNext = findAllJobsResult?.LastEvaluatedKey != null;
                            PagingViewModel.HasPrevious = responsePagingItem.LastViewedPageIndex > 0;
                            if (findAllJobsResult?.LastEvaluatedKey != null)
                                responsePagingItem.SetContinuationParameter(findAllJobsResult.LastEvaluatedKey);
                        });
                    });
            });
        }

        private void FindAll(JobListPagingItemViewModel pagingItem,
            JobListPagingItemViewModel.PageIndexType pageIndexType,
            Action<JobListPagingItemViewModel, GetJobsResult> completedAction)
        {
            var getJobsResult = FindAll(pagingItem, pageIndexType);
            if (getJobsResult == null)
            {
                completedAction?.Invoke(pagingItem, null);
            }
            else
            {
                var jobitems = getJobsResult.Jobs;
                _presetLoader.Load(new WeakAction<IList<Preset>>(presets =>
                    {
                        this.InvokeOnUi(() =>
                        {
                            AddToJobListItems(jobitems, presets);
                            completedAction?.Invoke(pagingItem, getJobsResult);
                        });
                    }),
                    true);
            }
        }

        private GetJobsResult FindAll(JobListPagingItemViewModel pagingItem,
            JobListPagingItemViewModel.PageIndexType pageIndexType)
        {
            pagingItem.GetContinuationParameter(pageIndexType);
            var topicArn = PipelineLoader.Instance.SelectedPipeline.Notifications
                .Completed;

            var jobList = _jobService.Get(new GetJobsParameter(RegionManager.Instance.Current.API,
                ParameterProvider.GetTopicNameFrom(topicArn), 
                pagingItem.GetContinuationParameter(pageIndexType)));

            //var jobList = _jobService.Get(RegionManager.Instance.Current.API, topicArn,
            //    pagingItem.GetContinuationParameter(pageIndexType));
            return new GetJobsResult(jobList.Jobs, jobList.LastEvaluatedKey);
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
            _browser.Main.JobSelector.LoadingManager.Show();
            try
            {
                await FindAllJobs(PagingItemViewModel, pageIndexType);
            }
            catch (TaskCanceledException)
            {
                _logger.Debug.Write("refresh task cancel!!!");
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.Debug.Write(e.Message);
                _browser.ShowConfirmWindow(
                    new ConfirmWindowParameter(Resource.CNT_WARNING,
                        Resource.MSG_CONFIRM_ELASTIC_TRANSCODER_AUTHORITY,
                        MessageBoxButton.OK));
            }
            finally
            {
                _browser.Main.JobSelector.LoadingManager.Hide();
            }
        }

        private async void AddToJobListItems(IEnumerable<Job> jobItems,
            IList<Preset> presets)
        {
            Jobs.Clear();

            if (!_cancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            await this.CreateTask(() =>
            {
                foreach (var job in jobItems)
                {
                    var jobListItemViewModel = new JobItemViewModel(_browser, job)
                    {
                        InputVideoName = job.Payload.Input.Key,
                        FinishTime = job.Timestamp,
                        Status = "Complete",
                        OutputKeyPrefix = job.Payload.OutputKeyPrefix
                    };

                    // Playlist에서 Format과 상대 경로를 저장.
                    if (job.Payload.Playlists != null)
                        foreach (var playlist in job.Payload.Playlists)
                        {
                            var jobOutput = new JobListItemOutputViewModel
                            {
                                DisplayMediaType = MediaType.AdaptiveStreaming,
                                OutputKeys = playlist.OutputKeys?.ToList(),
                                DisplayName = playlist.Format.ToDisplayValue(),
                                RelativePath = playlist.Name,
                                OutputKeyPrefix = job.Payload.OutputKeyPrefix,
                                Extension = playlist.Format.GetExtension(),
                                OutputStatus = "Complete"
                            };
                            jobOutput.Initialize();
                            jobListItemViewModel.Outputs.Add(jobOutput);
                        }

                    // job의 output를 기준으로 playlist의 outputkey와 매칭하여 presetId를 저장.
                    if (job.Payload.Outputs != null)
                        foreach (var output in job.Payload.Outputs)
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

                                    extension = (matchedPresetId?.Container)
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
                                    OutputKeyPrefix = job.Payload.OutputKeyPrefix,
                                    Extension = extension,
                                    OutputStatus = "Complete"
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

        private class GetJobsResult
        {
            public GetJobsResult(IEnumerable<Job> jobs, LastEvaluatedKey lastEvaluatedKey)
            {
                Jobs = jobs;
                LastEvaluatedKey = lastEvaluatedKey;
            }

            public IEnumerable<Job> Jobs { get; }
            public LastEvaluatedKey LastEvaluatedKey { get; }
        }
    }
}