using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Repository;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Security.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Newtonsoft.Json;
using Unity;

// ReSharper disable InconsistentNaming

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class SignInViewModel : ViewModelBase
    {
        private const string AUTHORIZATION_ENDPOINT = "https://megaone.io";

        private readonly IBrowser _browser;

        private readonly ICloudMediaService _cloudMediaService;

        private readonly ConfigHolder _config;
        private readonly ILogger _logger;

        private readonly object SYNC_OBJECT = new object();
        private Authorization _authorization;
        private ICommand _cancelProjectSelectCommand;
        private int _currentPageNumber;
        private IEnumerable<StageItemViewModel> _currentPageStageItems;
        private bool _isAutoLogin;
        private bool _isBusy;
        private bool _isCancelButtonVisible;
        private bool _isEmptyProjectPage;

        //ProjectSelectView 전용
        private bool _isLeftNavigateButtonVisible;
        private bool _isLoadingProjectPage;
        private bool _isNavigationBarVisible;
        private bool _isProjectViewVisible = true;
        private bool _isRightNavigateButtonVisible;
        private bool _isSignIn;
        private bool _isStartButtonVisible;
        private ICommand _leftSlideNavigateCommand;

        private ICommand _loadCommand;
        private string _loginId;
        private ICommand _logoutCommand;
        private ICommand _moveProjectStepCommand;
        private ICommand _navigatingCommand;

        private ICommand _projectSelectionChangedCommand;
        private ICommand _rightNavigateCommand;
        private ProjectItemViewModel _selectedProject;
        private StageItemViewModel _selectedStage;

        private StageItemViewModel _selectingStage;


        private bool _selectionsChangedFlag;

        private ICommand _selectProjectCommand;
        private List<StageItemViewModel> _stageItems;
        private ICommand _stagePerPageNumberChangedCommand;

        private int _stageTotal;
        private ICommand _startProjectCommand;
        private int _totalPage;
        private string _uriSource;

        public SignInViewModel(ICloudMediaService cloudMediaService, ILogger logger, IBrowser browser)
        {
            _logger = logger;
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _config = ConfigHolder.Current;

            CurrentPageNumber = 1;
            UriSource = "about:blank";
            LoadAuthorizationInfo();
        }

        private bool SelectionsChangedFlag
        {
            get
            {
                lock (SYNC_OBJECT)
                {
                    return _selectionsChangedFlag;
                }
            }
            set
            {
                lock (SYNC_OBJECT)
                {
                    _selectionsChangedFlag = value;
                }
            }
        }

        internal string AuthorizationFilePath => $"{Path.GetTempPath()}\\subtitleAuthorization.json";
        private UserProfile User { get; set; }

        public List<StageItemViewModel> StageItems
        {
            get => _stageItems;
            set => Set(ref _stageItems, value);
        }

        public IEnumerable<StageItemViewModel> CurrentPageStageItems
        {
            get => _currentPageStageItems;
            set => Set(ref _currentPageStageItems, value);
        }

        public StageItemViewModel SelectedStage
        {
            get => _selectedStage;
            set => Set(ref _selectedStage, value);
        }

        public StageItemViewModel SelectingStage
        {
            get => _selectingStage;
            set => Set(ref _selectingStage, value);
        }

        public ProjectItemViewModel SelectedProject
        {
            get => _selectedProject;
            set => Set(ref _selectedProject, value);
        }

        public bool IsProjectViewVisible
        {
            get => _isProjectViewVisible;
            set => Set(ref _isProjectViewVisible, value);
        }

        public bool IsSignIn
        {
            get => _isSignIn;
            set => Set(ref _isSignIn, value);
        }

        public bool IsPageChanged { get; set; }

        public bool IsLeftNavigateButtonVisible
        {
            get => _isLeftNavigateButtonVisible;
            set => Set(ref _isLeftNavigateButtonVisible, value);
        }

        public bool IsRightNavigateButtonVisible
        {
            get => _isRightNavigateButtonVisible;
            set => Set(ref _isRightNavigateButtonVisible, value);
        }

        public bool IsCancelButtonVisible
        {
            get => _isCancelButtonVisible;
            set => Set(ref _isCancelButtonVisible, value);
        }

        public bool IsStartButtonVisible
        {
            get => _isStartButtonVisible;
            set => Set(ref _isStartButtonVisible, value);
        }

        public bool IsNavigationBarVisible
        {
            get => _isNavigationBarVisible;
            set => Set(ref _isNavigationBarVisible, value);
        }

        public bool IsEmptyProjectPage
        {
            get => _isEmptyProjectPage;
            set => Set(ref _isEmptyProjectPage, value);
        }

        public bool IsLoadingProjectPage
        {
            get => _isLoadingProjectPage;
            set => Set(ref _isLoadingProjectPage, value);
        }

        public bool IsAutoLogin
        {
            get => _config.Subtitle.AutoLogin;
            set => Set(ref _isAutoLogin, value);
        }

        public string UriSource
        {
            get => _uriSource;
            set => Set(ref _uriSource, value);
        }

        public string LoginId
        {
            get => _loginId;
            set => Set(ref _loginId, value);
        }

        public int StageTotal
        {
            get => _stageTotal;
            set => Set(ref _stageTotal, value);
        }

        public int TotalPage
        {
            get => _totalPage;
            set => Set(ref _totalPage, value);
        }

        public int StagePerPageNumber { get; set; }

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set => Set(ref _currentPageNumber, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public ICommand MoveProjectStepCommand
        {
            get
            {
                return _moveProjectStepCommand = _moveProjectStepCommand ??
                                                 new RelayCommand(OnMoveProjectStep, CanExecuteMoveProjectStep);
            }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }


        public ICommand NavigatingCommand
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(OnNavigating); }
        }

        public ICommand LeftSlideNavigateCommand
        {
            get
            {
                return _leftSlideNavigateCommand =
                    _leftSlideNavigateCommand ?? new RelayCommand<string>(OnLeftSlideNavigate);
            }
        }

        public ICommand RightSlideNavigateCommand
        {
            get
            {
                return _rightNavigateCommand = _rightNavigateCommand ?? new RelayCommand<string>(OnRightSlideNavigate);
            }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand = _logoutCommand ?? new RelayCommand(OnLogout); }
        }

        public ICommand StagePerPageNumberChangedCommand
        {
            get
            {
                return _stagePerPageNumberChangedCommand =
                    _stagePerPageNumberChangedCommand ?? new RelayCommand<int>(OnStagePerPageNumberChanged);
            }
        }

        public ICommand StartProjectCommand
        {
            get
            {
                return _startProjectCommand =
                    _startProjectCommand ?? new RelayCommand(StartProject, CanStartProject);
            }
        }

        public ICommand CancelProjectSelectCommand
        {
            get
            {
                return _cancelProjectSelectCommand =
                    _cancelProjectSelectCommand ?? new RelayCommand(OnCancelProjectSelect);
            }
        }

        public ICommand SelectProjectCommand
        {
            get
            {
                return _selectProjectCommand =
                    _selectProjectCommand ?? new RelayCommand<ProjectItemViewModel>(SelectProject);
            }
        }

        public void Save()
        {
            _config.Subtitle.AutoLogin = _isAutoLogin;
            ConfigHolder.Save(_config);

            if (_isAutoLogin)
                SaveAuthorization();
        }


        public Authorization GetAuthorization()
        {
            // 유효성 검사.
            // 유효한 토큰인지 확인한다.
            // 유효하지 않다면, refresh token을 받도록 exception을 낸다.
            return _authorization;
        }

        private void OnStagePerPageNumberChanged(int obj)
        {
            StagePerPageNumber = obj;

            CalculateTotalPage();

            CalculateStageSlidePosition();
        }

        private void OnRightSlideNavigate(string obj)
        {
            ++CurrentPageNumber;

            CalculateStageSlidePosition();
        }

        private void OnLeftSlideNavigate(string obj)
        {
            if (CurrentPageNumber > 0)
                --CurrentPageNumber;

            CalculateStageSlidePosition();
        }

        private bool CanStartProject()
        {
            if (StageTotal == 0)
                return false;

            return SelectingStage?.SelectedProject != SelectedProject;
        }

        private bool CanExecuteMoveProjectStep()
        {
            return !IsProjectViewVisible;
        }

        private void OnMoveProjectStep()
        {
            IsProjectViewVisible = true;
            IsCancelButtonVisible = true;
            IsStartButtonVisible = true;
            CurrentPageNumber = 1;

            if (StageItems == null || SelectedProject == null) return;

            var projectItem = StageItems.SingleOrDefault(stage => stage.Id.Equals(SelectedProject.StageId));
            if (projectItem != null)
                projectItem.SelectedProject = SelectedProject;
        }

        private void CalculateTotalPage()
        {
            if (StageTotal == 0 || StagePerPageNumber == 0)
                return;

            TotalPage = Convert.ToInt32(Math.Ceiling(StageTotal / (decimal) StagePerPageNumber));

            IsNavigationBarVisible = TotalPage > 1;
        }


        private void CalculateStageSlidePosition()
        {
            if (StageItems == null) return;

            var startIndex = (CurrentPageNumber - 1) * StagePerPageNumber;
            var endIndex = startIndex + StagePerPageNumber;

            var newStageList = new List<StageItemViewModel>();
            for (var i = startIndex; i < endIndex; i++)
            {
                if (i >= StageItems.Count)
                    break;

                newStageList.Add(StageItems[i]);
            }

            //if (SelectingStage != null)
            //{
            //    SelectingStage.IsSelected = false;
            //    SelectingStage = null;
            //}

            CurrentPageStageItems = newStageList;

            if (TotalPage == 1)
            {
                IsRightNavigateButtonVisible = false;
                IsLeftNavigateButtonVisible = false;
            }
            else if (TotalPage > 1 && CurrentPageNumber == 1)
            {
                IsRightNavigateButtonVisible = true;
                IsLeftNavigateButtonVisible = false;
            }
            else if (TotalPage > 1 && CurrentPageNumber > 1)
            {
                IsRightNavigateButtonVisible = true;
                IsLeftNavigateButtonVisible = true;
            }
            else if (TotalPage == CurrentPageNumber)
            {
                IsRightNavigateButtonVisible = false;
                IsLeftNavigateButtonVisible = true;
            }
        }


        private void OnLogout()
        {
            IsProjectViewVisible = false;
            IsSignIn = false;

            StageItems = null;
            CurrentPageStageItems = null;

            SelectedProject = null;
            SelectedStage = null;

            SelectingStage = null;

            UriSource = AuthorizationRepository.LOGIN_URI;
        }

        private async void LoginByAuthorizationCodeAsync(string code)
        {
            try
            {
                _authorization = await _cloudMediaService.LoginByAuthorizationCodeAsync(code, CancellationToken.None);

                if (string.IsNullOrEmpty(_authorization?.AccessToken))
                    return;

                LoadStageAndProjectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
        }

        private async void LoadStageAndProjectAsync()
        {
            try
            {
                IsLoadingProjectPage = true;
                IsBusy = true;

                User = await _cloudMediaService.GetUserAsync(_authorization, CancellationToken.None);

                // 유저 인증 실패 401
                if (User == null)
                {
                    IsSignIn = false;
                    IsBusy = false;
                    UriSource = AuthorizationRepository.LOGIN_URI;
                    return;
                }

                LoginId = User.Username;

                IsProjectViewVisible = true;
                IsSignIn = true;

                StageItems = User.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                             new List<StageItemViewModel>();


                var emptyProjectStages = new List<StageItemViewModel>();

#if STAGING
                emptyProjectStages.AddRange(StageItems.Where(stage => !stage.ProjectItems?.Any() ?? true).ToList());
#else
                foreach (var stageItem in StageItems)
                {
                    var projects = await _cloudMediaService.GetProjects(
                        new GetProjectsParameter(_authorization, stageItem.Id, stageItem.Name),
                        CancellationToken.None);

                    if (projects == null || projects.TotalCount == 0)
                    {
                        emptyProjectStages.Add(stageItem);
                        continue;
                    }

                    var projectItems = projects.Results
                        .Select(project => new ProjectItemViewModel(stageItem.Id, project)).ToList();
                    stageItem.ProjectItems = projectItems;
                }

#endif

                foreach (var item in emptyProjectStages) StageItems.Remove(item);

                // Test Data 

                var firstItem = StageItems.First();

                var originalName = firstItem.Name;
                for (var i = 1; i < 7; i++)
                {
                    var newItem = new StageItemViewModel(firstItem.Source)
                    {
                        Id = $"test{i}",
                        Name = $"{originalName}_{i}"
                        //ProjectItems = firstItem.ProjectItems.Select(item => new ProjectItemViewModel($"test{i}", item.Source)).ToList()
                    };

                    var projectModelList = new List<ProjectItemViewModel>();
                    var count = 1;
                    foreach (var firstItemProjectItem in firstItem.ProjectItems)
                    {
                        var project = new Project(
                            $"{i}_{count}",
                            firstItemProjectItem.Source.Name,
                            firstItemProjectItem.Source.Description,
                            firstItemProjectItem.Source.UsePlayout,
                            firstItemProjectItem.Source.IsActive,
                            firstItemProjectItem.Source.CreatedAt,
                            firstItemProjectItem.Source.CreatedById,
                            firstItemProjectItem.Source.CreatedByName,
                            firstItemProjectItem.Source.CreatedByUsername,
                            firstItemProjectItem.Source.UpdatedAt,
                            firstItemProjectItem.Source.UpdatedById,
                            firstItemProjectItem.Source.UpdatedByName,
                            firstItemProjectItem.Source.UpdatedByUsername);

                        projectModelList.Add(new ProjectItemViewModel(newItem.Id, project));
                        count++;
                    }

                    newItem.ProjectItems = projectModelList;

                    StageItems.Add(newItem);
                }
                // ----

                StageTotal = StageItems.Count();

                CalculateTotalPage();

                if (StageTotal == 0)
                {
                    IsLoadingProjectPage = false;
                    IsEmptyProjectPage = true;
                }
                else
                {
                    IsLoadingProjectPage = false;
                    IsEmptyProjectPage = false;

                    CalculateStageSlidePosition();

                    if (IsAutoLogin)
                        SaveAuthorization();
                }

                IsBusy = false;
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }

        private void SaveAuthorization()
        {
            try
            {
                var profileData = JsonConvert.SerializeObject(_authorization).EncryptWithRfc2898("Megazone@1");

                File.WriteAllText(AuthorizationFilePath, profileData);
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }

        private void LoadAuthorizationInfo()
        {
            try
            {
                var profileData = File.ReadAllText(AuthorizationFilePath);
                _authorization =
                    JsonConvert.DeserializeObject<Authorization>(profileData.DecryptWithRfc2898("Megazone@1"));
            }
            catch (FileNotFoundException)
            {
                _authorization = null;
            }
        }

        public void ClearAuthorization()
        {
            _authorization = null;

            if (File.Exists(AuthorizationFilePath))
                File.Delete(AuthorizationFilePath);
        }

        private void Load()
        {
            if (!_config.Subtitle.AutoLogin)
            {
                UriSource = AuthorizationRepository.LOGIN_URI;
                return;
            }

            if (_authorization != null)
            {
                LoadStageAndProjectAsync();
            }
            else
            {
                UriSource = AuthorizationRepository.LOGIN_URI;
                IsProjectViewVisible = false;
                IsSignIn = false;
            }
        }

        private void StartProject()
        {
            Console.WriteLine($@"StagePerPageNumber : {StagePerPageNumber}");

            if (SelectedProject != null)
            {
                var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();

                var dirtyTabs = subtitleViewModel?.Tabs.Where(tab => tab.CheckDirty()).ToList();

                var isDirty = dirtyTabs != null && dirtyTabs.Any();
                if (isDirty)
                {
                    // [resource].
                    var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(
                        Resource.CNT_WARNING,
                        "프로젝트가 변경됩니다.\r\n이 작업으로 인해 기존 양식의 데이터를 손실 할 수 있습니다.\r\n\r\n계속하시겠습니까?",
                        MessageBoxButton.OKCancel));

                    if (result == MessageBoxResult.Cancel) return;

                    var removeTabs = subtitleViewModel.Tabs.ToList();

                    foreach (var tab in removeTabs)
                        MessageCenter.Instance.Send(
                            new Subtitle.DeleteTabMessage(this, tab as SubtitleTabItemViewModel));
                }
            }

            SelectedStage = SelectingStage;
            SelectedProject = SelectingStage.SelectedProject;
            IsProjectViewVisible = string.IsNullOrEmpty(SelectedProject?.ProjectId) ||
                                   string.IsNullOrEmpty(SelectedStage?.Id);
        }

        private void OnCancelProjectSelect()
        {
            IsProjectViewVisible = false;
            IsCancelButtonVisible = false;
            IsStartButtonVisible = false;
            CurrentPageNumber = 1;
        }

        private void OnNavigating(string code)
        {
            if (string.IsNullOrEmpty(code))
                return;

            LoginByAuthorizationCodeAsync(code);
        }


        private void SelectProject(ProjectItemViewModel projectItem)
        {
            if (!SelectionsChangedFlag)
            {
                SelectionsChangedFlag = true;

                SelectingStage = StageItems.Single(stage => stage.Id.Equals(projectItem.StageId));

                foreach (var stage in StageItems)
                    if (!stage.Equals(SelectingStage))
                        if (stage.SelectedProject != null)
                            stage.SelectedProject = null;
                SelectionsChangedFlag = false;
            }
        }
    }
}