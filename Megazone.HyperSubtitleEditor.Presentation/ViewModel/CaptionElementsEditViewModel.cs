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
using Megazone.HyperSubtitleEditor.Presentation;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class CaptionElementsEditViewModel : ViewModelBase
    {
        private bool _isShow;
        private bool _isLocalCaptionOnly;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private string _assetName;
        private string _assetId;
        private IEnumerable<CaptionKind> _subtitleKinds;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _applyCommand;
        private ICommand _closeCommand;


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

        public bool IsLocalCaptionOnly
        {
            get => _isLocalCaptionOnly;
            set => Set(ref _isLocalCaptionOnly, value);
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
        public ICommand CloseCommand
        {
            get => _closeCommand = _closeCommand ?? new RelayCommand(Close);
        }

        public CaptionElementsEditViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel,
            WorkBarViewModel workBarViewModel)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
            _workBarViewModel = workBarViewModel;

            AssetName = null;
            AssetId = null;

            MessageCenter.Instance.Regist<Message.View.CaptionElementsEditView.ChangedTabMessage>(OnChangedTab);

        }

        ~CaptionElementsEditViewModel()
        {
            MessageCenter.Instance.Unregist<Message.View.CaptionElementsEditView.ChangedTabMessage>(OnChangedTab);
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
            List<CaptionElementItemViewModel> newElementItemViewModel = null;

            var elementItemViewModelOfCloud = _workBarViewModel.CaptionAssetItem?.Elements?.ToList();
            
            if (elementItemViewModelOfCloud != null)
            {
                newElementItemViewModel = elementItemViewModelOfCloud;
                foreach (var elementItem in newElementItemViewModel)
                {
                    elementItem.CanDeploy = true;
                    elementItem.IsOpened = false;
                    elementItem.IsDirty = false;
                }

                IsLocalCaptionOnly = false;
            }
            else
            {
                newElementItemViewModel = new List<CaptionElementItemViewModel>();

                IsLocalCaptionOnly = true;
            }

            foreach (var tab in _subtitleViewModel.Tabs)
            {
                //elementItemViewModelOfCloud 가 null 이라면 tab에 있는 모든 caption element는 로컬 파일이고
                //newElementItem 에 도 null이 됨
                var newElementItem = elementItemViewModelOfCloud?.FirstOrDefault(e => e.Id == tab.Caption?.Id);

                if (newElementItem != null)
                {
                    newElementItem.IsOpened = true;
                    newElementItem.IsDirty = tab.IsDirty;
                }
                else
                {
                    var caption = new Caption(tab.Caption?.Id, false, false, tab.LanguageCode, tab.CountryCode,
                                tab.Kind.ToString().ToUpper(), tab.Name, tab.Caption?.Url, "", tab.Caption?.MimeType,
                                tab.Caption?.Size ?? 0);

                    newElementItem = new CaptionElementItemViewModel(caption)
                    {
                        IsOpened = true,
                        CanDeploy = false
                    };

                    newElementItemViewModel?.Add(newElementItem);
                }
            }

            return newElementItemViewModel;
        }

        private async void OnChangedTab(Message.View.CaptionElementsEditView.ChangedTabMessage message)
        {
            if (IsShow)
                CaptionItems = await MakeList();
        }
    }
}
