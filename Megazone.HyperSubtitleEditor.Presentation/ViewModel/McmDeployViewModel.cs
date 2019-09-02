using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class McmDeployViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;
        private CaptionAssetItemViewModel _captionAssetItem;
        private bool _captionCreateMode;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private ICommand _confirmCommand;
        private ICommand _loadCommand;
        private string _projectName;
        private string _stageName;
        private VideoItemViewModel _videoItem;

        public McmDeployViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel,
            IBrowser browser)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
            _browser = browser;
        }

        public Action CloseAction { get; set; }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
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

        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }

        public CaptionAssetItemViewModel CaptionAssetItem
        {
            get => _captionAssetItem;
            set => Set(ref _captionAssetItem, value);
        }

        public IEnumerable<CaptionElementItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }

        public bool CaptionCreateMode
        {
            get => _captionCreateMode;
            set => Set(ref _captionCreateMode, value);
        }

        private void Load()
        {
            try
            {
                CaptionCreateMode = _subtitleViewModel.WorkContext.OpenedCaptionAsset == null;
                StageName = _signInViewModel.SelectedStage.Name;
                ProjectName = _signInViewModel.SelectedProject.Name;
                VideoItem = new VideoItemViewModel(_subtitleViewModel.WorkContext.OpenedVideo);
                CaptionAssetItem = _subtitleViewModel.WorkContext.OpenedCaptionAsset != null
                    ? new CaptionAssetItemViewModel(_subtitleViewModel.WorkContext.OpenedCaptionAsset)
                    : null;
                CaptionItems = MakeList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            IEnumerable<CaptionElementItemViewModel> MakeList()
            {
                var captionList = _subtitleViewModel.Tabs.Select(tab => new CaptionElementItemViewModel(tab.Caption))
                    .ToList();
                foreach (var item in captionList)
                    item.IsSelected = true;

                return captionList;
            }
        }

        private bool CanConfirm()
        {
            return CaptionItems?.Where(x => x.IsSelected).Any() ?? false;
        }

        private void Confirm()
        {
            try
            {
                CloseAction?.Invoke();

                var video = VideoItem?.Source;
                var captionAsset = CaptionAssetItem?.Source;
                var selectedCaptionList = CaptionItems.Where(x => x.IsSelected).Select(item => item.Source).ToList();

                MessageCenter.Instance.Send(new Subtitle.McmDeployRequestedMessage(this,
                    new McmDeployRequestedMessageParameter(video, captionAsset, selectedCaptionList)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}