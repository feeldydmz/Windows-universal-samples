using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Api.Transcoder.Domain;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class McmDeployViewModel : ViewModelBase
    {
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        private string _assetName;
        private CaptionAssetItemViewModel _captionAssetItem;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private ICommand _confirmCommand;
        private ICommand _loadCommand;
        private string _projectName;
        private TrackKind _selectedSubtitleKind;
        private string _stageName;
        private IEnumerable<TrackKind> _subtitleKinds;
        private VideoItemViewModel _videoItem;

        public McmDeployViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
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

        public string AssetName
        {
            get => _assetName;
            set => Set(ref _assetName, value);
        }

        public IEnumerable<TrackKind> SubtitleKinds
        {
            get => _subtitleKinds;
            set => Set(ref _subtitleKinds, value);
        }

        public TrackKind SelectedSubtitleKind
        {
            get => _selectedSubtitleKind;
            set => Set(ref _selectedSubtitleKind, value);
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

        private void Load()
        {
            try
            {
                StageName = _signInViewModel.SelectedStage.Name;
                ProjectName = _signInViewModel.SelectedProject.Name;
                VideoItem = new VideoItemViewModel(_subtitleViewModel.WorkContext.OpenedVideo);
                if (_subtitleViewModel.WorkContext.OpenedCaptionAsset != null)
                    CaptionAssetItem = new CaptionAssetItemViewModel(_subtitleViewModel.WorkContext.OpenedCaptionAsset);
                CaptionItems = MakeList();

                // 에셋 생성모드
                if (_subtitleViewModel.WorkContext.OpenedCaptionAsset == null)
                {
                    SubtitleKinds = new List<TrackKind>
                    {
                        TrackKind.Subtitle,
                        TrackKind.Caption,
                        TrackKind.Chapter
                    };
                    SelectedSubtitleKind = Convert(CaptionItems.First().Kind);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            TrackKind Convert(string kind)
            {
                switch (kind?.ToUpper())
                {
                    case "CAPTION": return TrackKind.Caption;
                    case "CHAPTER": return TrackKind.Chapter;
                    case "DESCRIPTION": return TrackKind.Description;
                    case "METADATA": return TrackKind.Metadata;
                    case "SUBTITLE": return TrackKind.Subtitle;
                }

                return TrackKind.Subtitle;
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
            if (_subtitleViewModel.WorkContext.OpenedCaptionAsset == null)
                return !string.IsNullOrEmpty(AssetName) && (CaptionItems?.Where(x => x.IsSelected).Any() ?? false);
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