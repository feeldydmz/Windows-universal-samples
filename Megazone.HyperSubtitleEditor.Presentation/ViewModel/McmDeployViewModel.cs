using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class McmDeployViewModel : ViewModelBase
    {
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;

        private string _assetName;
        private CaptionAssetItemViewModel _captionAssetItem;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private ICommand _confirmCommand;
        private bool _hasCaptionAsset;

        private bool _hasVideo;
        private ICommand _loadCommand;
        private string _projectName;
        private CaptionKind _selectedSubtitleKind;
        private string _stageName;
        private IEnumerable<CaptionKind> _subtitleKinds;
        private VideoItemViewModel _videoItem;

        public McmDeployViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel,
            WorkBarViewModel workBarViewModel)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
            _workBarViewModel = workBarViewModel;
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

        //public IEnumerable<CaptionKind> SubtitleKinds
        //{
        //    get => _subtitleKinds;
        //    set => Set(ref _subtitleKinds, value);
        //}

        public CaptionKind SelectedSubtitleKind
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

        public bool HasVideo
        {
            get => _hasVideo;
            set => Set(ref _hasVideo, value);
        }

        public bool HasCaptionAsset
        {
            get => _hasCaptionAsset;
            set => Set(ref _hasCaptionAsset, value);
        }

        private async void Load()
        {
            try
            {
                StageName = _signInViewModel.SelectedStage.Name;
                ProjectName = _signInViewModel.SelectedProject.Name;
                VideoItem = _workBarViewModel.VideoItem;
                CaptionAssetItem = _workBarViewModel.CaptionAssetItem;
                AssetName = _workBarViewModel.CaptionAssetItem?.Name ?? "Untitle";

                HasVideo = !string.IsNullOrEmpty(_workBarViewModel.VideoItem?.Id);
                HasCaptionAsset = !string.IsNullOrEmpty(_workBarViewModel.CaptionAssetItem?.Id);

                CaptionItems = await MakeList();

                SelectedSubtitleKind = CaptionItems.First().Kind;

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<IEnumerable<CaptionElementItemViewModel>> MakeList()
        {
            // 현재 탭으로 오픈된 자막을 게시한다.
            CaptionElementItemViewModel CreateCaptionElementItemViewModel(ISubtitleTabItemViewModel tab)
            {
                var caption = new Caption(tab.Caption?.Id, false, false, tab.LanguageCode, tab.CountryCode,
                    tab.Kind.ToString().ToUpper(), tab.Name, tab.Caption?.Url, "", tab.Caption?.MimeType,
                    tab.Caption?.Size ?? 0);

                return new CaptionElementItemViewModel(caption)
                {
                    IsSelected = !string.IsNullOrEmpty(tab.Name) 
                                 && !string.IsNullOrEmpty(tab.LanguageCode) 
                                 && !string.IsNullOrEmpty(tab.CountryCode)
                                 && tab.IsDirty,

                    CanDeploy = !string.IsNullOrEmpty(tab.Name) && !string.IsNullOrEmpty(tab.LanguageCode) &&
                                !string.IsNullOrEmpty(tab.CountryCode)
                };
            }

            var editedCaptionList = _subtitleViewModel.Tabs.Select(CreateCaptionElementItemViewModel).ToList();

            //foreach (var item in editedCaptionList)
            //{
            //    item.IsSelected = true;
            //    item.CanDeploy = true;
            //}

            var captionAssetId = _workBarViewModel.CaptionAssetItem?.Id;
            if (!string.IsNullOrEmpty(captionAssetId))
            {
                var captionAsset = await _workBarViewModel.GetCaptionAssetAsync(captionAssetId);
                if (!string.IsNullOrEmpty(captionAsset?.Id))
                {
                    var captionItemList =
                        captionAsset.Elements?.Select(caption => new CaptionElementItemViewModel(caption)).ToList() ??
                        new List<CaptionElementItemViewModel>();

                    // 편집하지 않은 캡션 정보 추가.
                    foreach (var item in captionItemList)
                        if (!editedCaptionList.Any(caption => caption.Id?.Equals(item.Id) ?? false))
                            editedCaptionList.Add(item);
                }
            }

            return editedCaptionList;
        }

        private bool CanConfirm()
        {
            if (_workBarViewModel.CaptionAssetItem == null)
                return !string.IsNullOrEmpty(AssetName) && (CaptionItems?.Where(x => x.IsSelected).Any() ?? false);
            return CaptionItems?.Where(x => x.IsSelected).Any() ?? false;
        }

        private void Confirm()
        {
            try
            {
                CloseAction?.Invoke();
                var video = VideoItem?.Source;
                var captionAsset = CaptionAssetItem?.Source ?? CreateCaptionAsset();
                var selectedCaptionList = CaptionItems.Where(x => x.IsSelected).Select(item => item.Source).ToList();

                foreach (var caption in selectedCaptionList) caption.Kind = SelectedSubtitleKind.ToString().ToUpper();

                // TODO 여기서 captionAsset 을 생성해서 넘겨 줄 필요가 있나
                // 실제로 사용하는건 assetId 와 assetName 뿐
                MessageCenter.Instance.Send(new CloudMedia.DeployRequestedMessage(this,
                    new DeployRequestedMessageParameter(video, captionAsset, selectedCaptionList)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private CaptionAsset CreateCaptionAsset()
        {
            var selectedCaptionList = CaptionItems.Where(x => x.IsSelected).Select(item => item.Source).ToList();
            return new CaptionAsset(null, AssetName, "ACTIVE", "CAPTION", "TEXT", "DIRECT", 0, 1, null,
                selectedCaptionList);
        }
    }
}