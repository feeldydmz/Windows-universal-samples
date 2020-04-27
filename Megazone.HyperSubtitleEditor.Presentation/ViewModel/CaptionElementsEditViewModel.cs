using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class CaptionElementsEditViewModel : ViewModelBase
    {
        private bool _isShow;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private string _assetName;
        private string _assetId;
        private IEnumerable<CaptionKind> _subtitleKinds;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _applyCommand;


        public IEnumerable<CaptionElementItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }

        public bool IsShow
        {
            get => _isShow;
            set => Set(ref _isShow, value);
        }

        public string AssetName
        {
            get => _assetName;
            set => Set(ref _assetName, value);
        }

        public string AssetId
        {
            get => _assetId;
            set => Set(ref _assetId, value);
        }

        public IEnumerable<CaptionKind> SubtitleKinds
        {
            get => _subtitleKinds;
            set => Set(ref _subtitleKinds, value);
        }

        public ICommand ApplyCommand
        {
            get => _applyCommand = _applyCommand ?? new RelayCommand(Apply);
        }

        public CaptionElementsEditViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel,
            WorkBarViewModel workBarViewModel)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
            _workBarViewModel = workBarViewModel;

            AssetName = null;
            AssetId = null;

        }

        public async void Show()
        {
            //IsShow = true;

            AssetId = _workBarViewModel.CaptionAssetItem?.Id;
            AssetName = _workBarViewModel.CaptionAssetItem?.Name;

            CaptionItems =  await MakeList();
        }

        public void Close()
        {
            IsShow = false;
        }

        public void Apply()
        {
            var openItems = new List<Caption>();
            var closeItems= new List<SubtitleTabItemViewModel>();

            foreach (var item in CaptionItems)
            {
                if (item.IsOpened != item.IsSelected)
                {
                    // 새로 탭 열기
                    if (item.IsSelected)
                    {
                        openItems.Add(item.Source);
                        //MessageCenter.Instance.Send(
                        //    new Message.SubtitleEditor.CaptionElementOpenRequestedMessage(this,));

                        //Debug.WriteLine("새 탭 열기");
                    }
                    // 기존 탭 닫기
                    else
                    {

                        var tab = _subtitleViewModel.Tabs.FirstOrDefault(t => t.Caption?.Id == item.Id);

                        //MessageCenter.Instance.Send(new Message.SubtitleEditor.CloseTabMessage(this, tab as SubtitleTabItemViewModel));
                        if(tab is SubtitleTabItemViewModel tabItem)
                            closeItems.Add(tabItem);

                        item.IsOpened = false;

                        //Debug.WriteLine("기존 탭 닫기");
                    }
                }

                Close();
            }

            MessageCenter.Instance.Send(
                new Message.SubtitleEditor.CaptionElementOpenRequestedMessage(this, openItems, closeItems));

        }

        private async Task<IEnumerable<CaptionElementItemViewModel>> MakeList()
        {
            //// 현재 탭으로 오픈된 자막을 게시한다.
            //CaptionElementItemViewModel CreateCaptionElementItemViewModel(ISubtitleTabItemViewModel tab)
            //{
            //    var caption = new Caption(tab.Caption?.Id, false, false, tab.LanguageCode, tab.CountryCode,
            //        tab.Kind.ToString().ToUpper(), tab.Name, tab.Caption?.Url, "", tab.Caption?.MimeType,
            //        tab.Caption?.Size ?? 0);

            //    return new CaptionElementItemViewModel(caption)
            //    {
            //        IsSelected = !string.IsNullOrEmpty(tab.Name)
            //                     && !string.IsNullOrEmpty(tab.LanguageCode)
            //                     && !string.IsNullOrEmpty(tab.CountryCode),

            //        CanDeploy = !string.IsNullOrEmpty(tab.Name) 
            //                    && !string.IsNullOrEmpty(tab.LanguageCode)
            //                    && !string.IsNullOrEmpty(tab.CountryCode)
            //    };
            //}

            //var editedCaptionList = _subtitleViewModel.Tabs.Select(CreateCaptionElementItemViewModel).ToList();

            var tempCaptionList = _workBarViewModel.CaptionAssetItem.Elements.ToList();

            foreach (var element in tempCaptionList)
            {
                var result = _subtitleViewModel.Tabs.FirstOrDefault(t => t.Caption?.Id == element.Id);

                //var item = tempCaptionList.FirstOrDefault(c => c.Id == tab.Caption?.Id);
                element.CanDeploy = true;

                if (result != null)
                {
                    element.IsOpened = true;
                }
                else
                {
                    element.IsOpened = false;
                }
            }

            return tempCaptionList;

            //foreach (var item in editedCaptionList)
            //{
            //    item.IsSelected = true;
            //    item.CanDeploy = true;
            //}


            //if (!string.IsNullOrEmpty(AssetId))
            //{
            //    var captionAsset = await _workBarViewModel.GetCaptionAssetAsync(AssetId);
            //    if (!string.IsNullOrEmpty(captionAsset?.Id))
            //    {
            //        var captionItemList =
            //            captionAsset.Elements?.Select(caption => new CaptionElementItemViewModel(caption)).ToList() ??
            //            new List<CaptionElementItemViewModel>();

            //        // 편집하지 않은 캡션 정보 추가.
            //        foreach (var item in captionItemList)
            //            if (!editedCaptionList.Any(caption => caption.Id?.Equals(item.Id) ?? false))
            //                editedCaptionList.Add(item);
            //    }
            //}

            //return editedCaptionList;
        }
        //public ICommand OpenMetadataPopupCommand
        //{
        //    get
        //    {
        //        return _openCaptionElementsEditCommand =
        //            _openCaptionElementsEditCommand ?? new RelayCommand(Open);
        //    }
        //}


    }
}
