using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class CaptionAssetListViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly LanguageLoader _languageLoader;
        private readonly SignInViewModel _signInViewModel;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IEnumerable<CaptionAssetItemViewModel> _captionAssetItems;

        private ICommand _captionAssetSectionChangedCommand;
        private IEnumerable<DisplayItem> _captionKindItems;
        private ICommand _captionSelectionChangedCommand;
        private ICommand _confirmCommand;
        private ICommand _enterCommand;
        private ICommand _initializeCommand;
        private bool _isBusy;
        private bool _isConfirmButtonVisible;
        private bool _isInitialized;
        private bool _isLoading;
        private string _keyword;
        private IEnumerable<DisplayItem> _keywordTypeItems;
        private string _label;

        private IEnumerable<LanguageItem> _languages;
        private ICommand _loadCommand;
        private ICommand _refreshCommand;
        private ICommand _searchCommand;
        private CaptionAssetItemViewModel _selectedCaptionAssetItem;
        private DisplayItem _selectedCaptionKindItem;
        private DisplayItem _selectedKeywordType;

        private LanguageItem _selectedLanguage;
        private int _selectedPageNo = 1;

        private ICommand _selectedPageNoChangedCommand;

        private int _totalCount;

        public CaptionAssetListViewModel(IBrowser browser, ICloudMediaService cloudMediaService,
            SignInViewModel signInViewModel, LanguageLoader languageLoader)
        {
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
            _languageLoader = languageLoader;
        }

        public ICommand CaptionAssetSectionChangedCommand
        {
            get
            {
                return _captionAssetSectionChangedCommand =
                    _captionAssetSectionChangedCommand ?? new RelayCommand(OnCaptionAssetSectionChanged);
            }
        }

        public ICommand SelectedPageNoChangedCommand
        {
            get
            {
                return _selectedPageNoChangedCommand =
                    _selectedPageNoChangedCommand ?? new RelayCommand<int>(OnSelectedPageNoChanged);
            }
        }

        public ICommand InitializeCommand
        {
            get { return _initializeCommand = _initializeCommand ?? new RelayCommand(Initialize); }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand = _searchCommand ?? new RelayCommand<string>(Search); }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand = _refreshCommand ?? new RelayCommand(Refresh); }
        }

        public ICommand EnterCommand
        {
            get { return _enterCommand = _enterCommand ?? new RelayCommand<string>(Enter); }
        }

        public ICommand CaptionSelectionChangedCommand
        {
            get
            {
                return _captionSelectionChangedCommand = _captionSelectionChangedCommand ??
                                                         new RelayCommand<CaptionElementItemViewModel>(
                                                             OnCaptionSelectionChanged, CanCaptionSelectionChanged);
            }
        }

        public bool IsConfirmButtonVisible
        {
            get => _isConfirmButtonVisible;
            set => Set(ref _isConfirmButtonVisible, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public CaptionAssetItemViewModel SelectedCaptionAssetItem
        {
            get => _selectedCaptionAssetItem;
            set => Set(ref _selectedCaptionAssetItem, value);
        }

        public IEnumerable<CaptionAssetItemViewModel> CaptionAssetItems
        {
            get => _captionAssetItems;
            set => Set(ref _captionAssetItems, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => Set(ref _totalCount, value);
        }

        public int SelectedPageNo
        {
            get => _selectedPageNo;
            set => Set(ref _selectedPageNo, value);
        }

        public IEnumerable<DisplayItem> KeywordTypeItems
        {
            get => _keywordTypeItems;
            set => Set(ref _keywordTypeItems, value);
        }

        public DisplayItem SelectedKeywordType
        {
            get => _selectedKeywordType;
            set => Set(ref _selectedKeywordType, value);
        }

        public IEnumerable<DisplayItem> CaptionKindItems
        {
            get => _captionKindItems;
            set => Set(ref _captionKindItems, value);
        }

        public DisplayItem SelectedCaptionKindItem
        {
            get => _selectedCaptionKindItem;
            set => Set(ref _selectedCaptionKindItem, value);
        }

        public string Keyword
        {
            get => _keyword;
            set => Set(ref _keyword, value);
        }

        public string Label
        {
            get => _label;
            set => Set(ref _label, value);
        }

        public IEnumerable<LanguageItem> Languages
        {
            get => _languages;
            set => Set(ref _languages, value);
        }

        public LanguageItem SelectedLanguage
        {
            get => _selectedLanguage;
            set => Set(ref _selectedLanguage, value);
        }

        private async void OnSelectedPageNoChanged(int selectedPageNo)
        {
            if (_isLoading)
                return;
            await SearchAsync(Keyword, selectedPageNo - 1, true);
        }

        private void OnCaptionAssetSectionChanged()
        {
            SelectedCaptionAssetItem?.SelectAll();
            if (CaptionAssetItems != null)
                foreach (var captionAssetItem in CaptionAssetItems)
                    if (!captionAssetItem.Equals(SelectedCaptionAssetItem))
                        captionAssetItem.Initialize();

            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanCaptionSelectionChanged(CaptionElementItemViewModel arg)
        {
            return SelectedCaptionAssetItem?.Elements?.Any(element => element.Equals(arg)) ?? false;
        }

        private void Initialize()
        {
            _isInitialized = false;
            KeywordTypeItems = new List<DisplayItem>
            {
                new DisplayItem(Resource.CNT_NAME, "name"),
                new DisplayItem("Asset ID", "id")
            };
            CaptionKindItems = new List<DisplayItem>
            {
                new DisplayItem(string.Empty, string.Empty),
                new DisplayItem("Caption", CaptionKind.Caption.ToString()),
                new DisplayItem("Chapter", CaptionKind.Chapter.ToString()),
                new DisplayItem("Subtitle", CaptionKind.Subtitle.ToString()),
                new DisplayItem("Description", CaptionKind.Description.ToString()),
                new DisplayItem("Metadata", CaptionKind.Metadata.ToString())
            };

            if (SelectedKeywordType == null)
                SelectedKeywordType = KeywordTypeItems.First();
            SelectedPageNo = 1;
            _isInitialized = true;
        }

        public void ClearSearchParameter()
        {
            Keyword = "";
            SelectedCaptionKindItem = null;
            Label = "";
            SelectedLanguage = null;
        }

        private async void Load()
        {
            if (!_isInitialized)
                Initialize();

            if (_isLoading)
                return;

            _isLoading = true;

            if (!_languageLoader.Languages?.Any() ?? true)
                await _languageLoader.LoadAsync();

            var languageList = _languageLoader.Languages.ToList();
            languageList.Insert(0, new LanguageItem(null));
            Languages = languageList;

            await SearchAsync(Keyword, 0);
            _isLoading = false;
        }

        private async void Refresh()
        {
            await SearchAsync(Keyword, 0);
        }

        private async void Enter(string keyword)
        {
            await SearchAsync(keyword, 0);
        }

        private async void Search(string keyword)
        {
            await SearchAsync(keyword, 0);
        }

        private async Task SearchAsync(string keyword, int pageIndex, bool isPaging = false)
        {
            var kinds = SelectedCaptionKindItem != null ? new[] {SelectedCaptionKindItem.Key} : null;
            var conditions = MakeSearchConditions(Keyword, kinds, Label, SelectedLanguage);
            await SearchCaptionAssetAsync(pageIndex, conditions, isPaging);
        }

        private async Task SearchCaptionAssetAsync(int pageIndex, Dictionary<string, string> conditions, bool isPaging)
        {
            if (IsBusy)
                return;

            ValidCancellationTokenSource();
            IsBusy = true;
            try
            {
                if (!isPaging)
                {
                    SelectedPageNo = 1;
                    TotalCount = 0;
                    CaptionAssetItems = null;
                }

                SelectedCaptionAssetItem = null;
                var results = await GetCaptionAssetListAsync(new Pagination(pageIndex), conditions,
                    _cancellationTokenSource.Token);

                if (results == null) return;

                TotalCount = results.TotalCount;
                CaptionAssetItems = new ObservableCollection<CaptionAssetItemViewModel>(
                    results.List?.Select(captionAsset => new CaptionAssetItemViewModel(captionAsset)).ToList() ??
                    new List<CaptionAssetItemViewModel>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<CaptionAssetList> GetCaptionAssetListAsync(Pagination pagination,
            Dictionary<string, string> conditions,
            CancellationToken cancellationToken)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;

                if (string.IsNullOrEmpty(stageId) || string.IsNullOrEmpty(projectId) ||
                    string.IsNullOrEmpty(authorization?.AccessToken))
                    return null;

                return await _cloudMediaService.GetCaptionAssetsAsync(
                    new GetAssetsParameter(authorization, stageId, projectId, pagination, conditions),
                    cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private Dictionary<string, string> MakeSearchConditions(string keyword, IEnumerable<string> kinds, string label,
            LanguageItem language)
        {
            // 검색조건
            var conditions = new Dictionary<string, string>
            {
                // 자막 검색 기본 설정
                {"type", "CAPTION"}, {"hasAssociations", "true"}, {"status", "ACTIVE"}
            };

            if (!string.IsNullOrEmpty(keyword))
                conditions.Add(SelectedKeywordType.Key, keyword);

            var kindList = kinds?.ToList() ?? new List<string>();
            if (kindList.Any())
            {
                var values = new StringBuilder();
                var count = 0;
                foreach (var kind in kindList.Where(kind => !string.IsNullOrEmpty(kind)))
                {
                    if (count > 0)
                        values.Append(",");
                    values.Append(kind);
                    count++;
                }

                var valuesString = values.ToString();
                if (!string.IsNullOrEmpty(valuesString))
                    conditions.Add("kinds", valuesString);
            }


            if (!string.IsNullOrEmpty(label))
                conditions.Add("label", label);

            //if (!string.IsNullOrEmpty(language))
            //{
            //    var array = language.Split('-');
            //    if (array.Length == 2)
            //    {
            //        var languageCode = array[0];
            //        var countryCode = array[1];
            //        conditions.Add("language", languageCode);
            //        conditions.Add("country", countryCode);
            //    }
            //}

            if (!string.IsNullOrEmpty(language?.Code))
            {
                var languageCode = language.Code.Split('-')[0];
                var countryCode = language.Code.Split('-')[1];
                conditions.Add("language", languageCode);
                conditions.Add("country", countryCode);
            }

            return conditions;
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            if (SelectedCaptionAssetItem == null)
                return;

            var captionAssetItem = CaptionAssetItems.SingleOrDefault(assetItem =>
                assetItem.Elements?.Any(element => element.Equals(item)) ?? false);

            if (!SelectedCaptionAssetItem?.Equals(captionAssetItem) ?? true)
            {
                CaptionAssetItems?.ToList().ForEach(asset =>
                {
                    if (!asset.Equals(captionAssetItem))
                        asset.Initialize();
                });

                if (SelectedCaptionAssetItem != captionAssetItem)
                    SelectedCaptionAssetItem = captionAssetItem;
            }

            var isAnySeleted = captionAssetItem.Elements?.Any(element => element.IsSelected);

            if (isAnySeleted == false) SelectedCaptionAssetItem = null;
        }

        private bool CanConfirm()
        {
            return !IsBusy && SelectedCaptionAssetItem != null;
        }

        private void Confirm()
        {
            var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            var tabList = subtitleVm.Tabs?.ToList() ?? new List<ISubtitleTabItemViewModel>();
            if (subtitleVm.Tabs?.Any() ?? false)
            {
                if (subtitleVm.Tabs.Any(tab => tab.CheckDirty()))
                {
                    if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING, 
                            Resource.MSG_THERE_IS_WORK_IN_PROGRESS,
                            MessageBoxButton.OKCancel, 
                            Application.Current.MainWindow)) == MessageBoxResult.Cancel)
                        return;
                }

                var removeTabs = subtitleVm.Tabs.ToList();
                foreach (var tab in removeTabs)
                    MessageCenter.Instance.Send(
                        new Subtitle.CloseTabMessage(this, tab as SubtitleTabItemViewModel));
            }

            var asset = SelectedCaptionAssetItem?.Source;
            var selectedCaptionList =
                SelectedCaptionAssetItem?.Elements?.Where(caption => caption.IsSelected).Select(itemVm => itemVm.Source)
                    .ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new CloudMedia.CaptionOpenRequestedMessage(this,
                new CaptionOpenMessageParameter(null, asset, selectedCaptionList, true)));

            MessageCenter.Instance.Send(new LeftSideMenu.CloseMessage(this));
        }

        private void ValidCancellationTokenSource()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}