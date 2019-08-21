using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using System;
using System.Windows.Input;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class McmDeployConfirmViewModel: ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private ICommand _loadCommand;
        private ICommand _confirmCommand;
        private string _stageName;
        private string _projectName;

        public Action CloseAction { get; set; }

        public McmDeployConfirmViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        
        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }
        
        public ICommand ConfrimCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm); }
        }

        public string StageName
        {
            get => _stageName;
            set => Set(ref _stageName, value);
        }

        public string ProjectName
        {
            get => _projectName;
            set => Set(ref _projectName, value);
        }

        private void Load()
        {
            //var videoItem = new VideoItemViewModel(WorkContext.Video);
        }

        private void Confirm()
        {
        }
    }
}
