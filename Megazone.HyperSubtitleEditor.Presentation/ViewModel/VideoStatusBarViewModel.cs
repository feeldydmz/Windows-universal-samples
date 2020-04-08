using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.View;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class VideoStatusBarViewModel : ViewModelBase
    {
        private ICommand _loadOriginalVideoCommand;
        private bool _isPlayPreviewVideo = false;

        private WorkBarViewModel _workbar;

        //public bool IsPreview => _mediaPlayer.IsLocalFile;

        internal VideoStatusBarViewModel()
        {
            _workbar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        }

        private bool _isPreview;

        public bool IsPreview
        {
            get => _isPreview;
            set => Set(ref _isPreview, value);
        }


        public ICommand LoadOriginalVideoCommand
        {
            get
            {
                return _loadOriginalVideoCommand = _loadOriginalVideoCommand ?? new RelayCommand(OnLoadOriginalVideo);
            }
        }

        //public bool IsPlayPreviewVideo => _mediaPlayer.IsLocalFile;


        private void OnLoadOriginalVideo()
        {

        }
    }
}
