using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class CaptionAssetMenuViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly LanguageLoader _languageLoader;
        private readonly SignInViewModel _signInViewModel;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CaptionAssetListViewModel _captionAssetList;

        private IEnumerable<DisplayItem> _captionKindItems;
        private ICommand _confirmCommand;
        private ICommand _enterCommand;
        private string _idOfSearch;
        private ICommand _initializeCommand;
        private bool _isAdvancedSearch;
        private bool _isBusy;
        private bool _isConfirmButtonVisible;
        private bool _isInitialized;
        private bool _isLoading;
        private string _keyword;
        private IEnumerable<DisplayItem> _keywordTypeItems;
        private string _labelOfSearch;

        private IEnumerable<LanguageItem> _languages;
        private ICommand _loadCommand;
        private string _nameOfSearch;
        private ICommand _refreshCommand;
        private ICommand _searchCommand;
        private ICommand _searchOptionChangeCommand;
        private DisplayItem _selectedCaptionKindItem;
        private DisplayItem _selectedKeywordType;

        private LanguageItem _selectedLanguage;
        private int _selectedPageNo = 1;

        private ICommand _selectedPageNoChangedCommand;

        private int _totalCount;

        public CaptionAssetMenuViewModel(IBrowser browser, ICloudMediaService cloudMediaService,
            SignInViewModel signInViewModel, LanguageLoader languageLoader)
        {
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
            _languageLoader = languageLoader;

            CaptionAssetList = new CaptionAssetListViewModel();
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

        public ICommand SearchOptionChangeCommand
        {
            get
            {
                return _searchOptionChangeCommand = _searchOptionChangeCommand ?? new RelayCommand(ChangedSearchOption);
            }
        }


        public ICommand RefreshCommand
        {
            get { return _refreshCommand = _refreshCommand ?? new RelayCommand(Refresh); }
        }

        public ICommand EnterCommand
        {
            get { return _enterCommand = _enterCommand ?? new RelayCommand<string>(Enter); }
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

        public bool IsAdvancedSearch
        {
            get => _isAdvancedSearch;
            set => Set(ref _isAdvancedSearch, value);
        }

        public CaptionAssetItemViewModel SelectedCaptionAssetItem => CaptionAssetList.SelectedCaptionAssetItem;

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

        public string LabelOfSearch
        {
            get => _labelOfSearch;
            set => Set(ref _labelOfSearch, value);
        }

        public string NameOfSearch
        {
            get => _nameOfSearch;
            set => Set(ref _nameOfSearch, value);
        }

        public string IdOfSearch
        {
            get => _idOfSearch;
            set => Set(ref _idOfSearch, value);
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

        public CaptionAssetListViewModel CaptionAssetList
        {
            get => _captionAssetList;
            set => Set(ref _captionAssetList, value);
        }

        private async void OnSelectedPageNoChanged(int selectedPageNo)
        {
            if (_isLoading)
                return;
            await SearchAsync(selectedPageNo - 1, true);
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
                new DisplayItem("SubtitleEditor", CaptionKind.Subtitle.ToString()),
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
            LabelOfSearch = "";
            IdOfSearch = "";
            NameOfSearch = "";
            SelectedLanguage = null;
        }

        public void Close()
        {
            ClearSearchParameter();
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

            await SearchAsync(0);
            _isLoading = false;
        }

        private async void Refresh()
        {
            await SearchAsync(0);
        }

        private async void Enter(string keyword)
        {
            await SearchAsync(0);
        }

        private async void Search(string keyword)
        {
            await SearchAsync(0);
        }

        private void ChangedSearchOption()
        {
            IsAdvancedSearch = !IsAdvancedSearch;

            ClearSearchParameter();
        }

        private async Task SearchAsync(int pageIndex, bool isPaging = false)
        {
            var kinds = SelectedCaptionKindItem != null ? new[] {SelectedCaptionKindItem.Key} : null;
            var conditions = MakeSearchConditions(Keyword,
                NameOfSearch,
                IdOfSearch,
                kinds,
                LabelOfSearch,
                SelectedLanguage);
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
                }

                CaptionAssetList.SelectedCaptionAssetItem = null;

                var results = await GetCaptionAssetListAsync(new Pagination(pageIndex), conditions,
                    _cancellationTokenSource.Token);

                if (results == null) return;

                TotalCount = results.TotalCount;
                var captionAssetItems = new ObservableCollection<CaptionAssetItemViewModel>(
                    results.List?.Select(captionAsset => new CaptionAssetItemViewModel(captionAsset)).ToList() ??
                    new List<CaptionAssetItemViewModel>());


                CaptionAssetList.CaptionAssetItems = captionAssetItems;
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

        private Dictionary<string, string> MakeSearchConditions(string keyword, string name, string id,
            IEnumerable<string> kinds, string label,
            LanguageItem language)
        {
            // 검색조건
            var conditions = new Dictionary<string, string>
            {
                // 자막 검색 기본 설정
                {"type", "CAPTION"}, {"hasAssociations", "true"}, {"status", "ACTIVE"}
            };

            if (!IsAdvancedSearch)
            {
                conditions.Add("q", Keyword);
            }
            else
            {
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

                if (!string.IsNullOrEmpty(name))
                    conditions.Add("name", name);

                if (!string.IsNullOrEmpty(id))
                    conditions.Add("id", id);

                if (!string.IsNullOrEmpty(label))
                    conditions.Add("label", label);

                if (!string.IsNullOrEmpty(language?.Code))
                {
                    var languageCode = language.Code.Split('-')[0];
                    var countryCode = language.Code.Split('-')[1];
                    conditions.Add("language", languageCode);
                    conditions.Add("country", countryCode);
                }
            }

            return conditions;
        }

        private bool CanConfirm()
        {
            return !IsBusy && SelectedCaptionAssetItem != null;
        }

        private void Confirm()
        {
            var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            //var tabList = subtitleVm.Tabs?.ToList() ?? new List<ISubtitleTabItemViewModel>();

            var asset = SelectedCaptionAssetItem?.Source;
            var selectedCaptionList =
                SelectedCaptionAssetItem?.Elements?.Where(caption => caption.IsSelected).Select(itemVm => itemVm.Source)
                    .ToList() ?? new List<Caption>();

            if (subtitleViewModel.Tabs?.Any() ?? false)
            {
                if (subtitleViewModel.Tabs.Any(tab => tab.CheckDirty()))
                {
                    var result = _browser.Main.ShowCreateWorkspaceConfirmWindow(Resource.CNT_OPEN);

                    if (!result.HasValue) return;

                    if (result.Value)
                    {
                        var param = new CreateNewWindowMessageParameter(null, asset, selectedCaptionList);
                        MessageCenter.Instance.Send(new Message.SubtitleEditor.CreateNewWindowMessage(this, param));

                        MessageCenter.Instance.Send(new LeftSideMenu.CloseMessage(this));

                        return;
                    }
                }

                MessageCenter.Instance.Send(new Message.SubtitleEditor.CleanUpSubtitleMessage(this));
            }

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