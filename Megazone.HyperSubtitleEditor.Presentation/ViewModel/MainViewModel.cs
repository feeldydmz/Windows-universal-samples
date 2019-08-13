using System.IO;
using System.Windows.Input;
using Megazone.Api.Transcoder.ServiceInterface;
using Megazone.Core.IoC;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class MainViewModel : ViewModelBase
    {
        private readonly IJobService _jobService;
        private string _jobId;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public MainViewModel(IJobService jobService)
        {
            _jobService = jobService;
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand UnloadedCommand
        {
            get { return _unloadedCommand = _unloadedCommand ?? new RelayCommand(OnUnloaded); }
        }

        public string JobId
        {
            get => _jobId;
            set => Set(ref _jobId, value);
        }

        private void OnLoaded()
        {
            MessageCenter.Instance.Regist<ReinitializeAppContextMessage>(OnReinitializeAppContextMessageReceived);
            MessageCenter.Instance.Regist<JobFoundMessage>(OnJobFound);
            CleanUpTempFiles();
        }

        private void CleanUpTempFiles()
        {
            this.InvokeOnTask(() =>
            {
                try
                {
                    var folderPath = this.GetTempFolderPath();
                    var di = new DirectoryInfo(folderPath);
                    if (di.Exists)
                        di.Delete(true);
                }
                catch
                {
                    // ignored
                }
            });
        }

        private void OnJobFound(JobFoundMessage message)
        {
            AppContext.SetJob(message.Job);
            JobId = message.Job.Payload.JobId;
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnReinitializeAppContextMessageReceived(ReinitializeAppContextMessage message)
        {
            if (string.IsNullOrEmpty(message.ProfileId) ||
                string.IsNullOrEmpty(message.PipelineId) ||
                string.IsNullOrEmpty(message.JobId))
            {
                AppContext.SetJob(null);
                JobId = null;
                return;
            }

            var appContext = new AppContext();
            appContext.SetConfig(message.ProfileId, message.PipelineId, message.JobId, message.Region);
            appContext.Initialize(_jobService, null);
        }

        private void OnUnloaded()
        {
            MessageCenter.Instance.Unregist<JobFoundMessage>(OnJobFound);
            MessageCenter.Instance.Unregist<ReinitializeAppContextMessage>(OnReinitializeAppContextMessageReceived);
        }
    }
}