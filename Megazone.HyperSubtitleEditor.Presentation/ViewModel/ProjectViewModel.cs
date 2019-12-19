using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Newtonsoft.Json;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class ProjectViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly LanguageLoader _languageLoader;
        private readonly ILogger _logger;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _closeCommand;
        private int _currentPageNumber;
        private IEnumerable<StageItemViewModel> _currentPageStageItems;
        private ProjectItemViewModel _defaultProject;
        private bool _hasRegisteredMessageHandlers;
        private bool _isBusy;
        private bool _isCancelButtonVisible;
        private bool _isEmptyProjectPage;
        private bool _isLeftNavigateButtonVisible;
        private bool _isLoadingProjectPage;
        private bool _isNavigationBarVisible;
        private bool _isProjectViewVisible = true;
        private bool _isRightNavigateButtonVisible;
        private bool _isStartButtonVisible;
        private ICommand _leftSlideNavigateCommand;
        private ICommand _loadCommand;
        private ICommand _rightNavigateCommand;
        private StageItemViewModel _selectedStage;
        private ICommand _selectProjectCommand;
        private ICommand _setDefaultProjectCommand;
        private List<StageItemViewModel> _stageItems;
        private ICommand _stagePerPageNumberChangedCommand;
        private int _stageTotal;
        private int _totalPage;
        private ICommand _unloadCommand;

        public ProjectViewModel(IBrowser browser, ICloudMediaService cloudMediaService, ILogger logger,
            SignInViewModel signInViewModel, LanguageLoader languageLoader)
        {
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _languageLoader = languageLoader;
            _logger = logger;
            _signInViewModel = signInViewModel;
            CurrentPageNumber = 1;
        }

        internal string DefaultProjectInfoFilePath => $"{Path.GetTempPath()}subtitleDefaultInfo.dat";

        public bool IsProjectViewVisible
        {
            get => _isProjectViewVisible;
            set => Set(ref _isProjectViewVisible, value);
        }

        public StageItemViewModel SelectedStage
        {
            get => _selectedStage;
            set => Set(ref _selectedStage, value);
        }

        public ProjectItemViewModel DefaultProject
        {
            get => _defaultProject;
            set => Set(ref _defaultProject, value);
        }

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

        public int TotalPage
        {
            get => _totalPage;
            set => Set(ref _totalPage, value);
        }

        public int StageTotal
        {
            get => _stageTotal;
            set => Set(ref _stageTotal, value);
        }

        public int StagePerPageNumber { get; set; }

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set => Set(ref _currentPageNumber, value);
        }

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

        public ICommand SelectProjectCommand
        {
            get
            {
                return _selectProjectCommand =
                    _selectProjectCommand ?? new RelayCommand<ProjectItemViewModel>(StartProject);
            }
        }

        public ICommand StagePerPageNumberChangedCommand
        {
            get
            {
                return _stagePerPageNumberChangedCommand =
                    _stagePerPageNumberChangedCommand ?? new RelayCommand<int>(OnStagePerPageNumberChanged);
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand =
                    _closeCommand ?? new RelayCommand(Close);
            }
        }

        public ICommand LeftSlideNavigateCommand
        {
            get
            {
                return _leftSlideNavigateCommand =
                    _leftSlideNavigateCommand ?? new RelayCommand<string>(NavigateLeft);
            }
        }
        public ICommand RightSlideNavigateCommand
        {
            get { return _rightNavigateCommand = _rightNavigateCommand ?? new RelayCommand<string>(NavigateRight); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }
        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand UnloadCommand
        {
            get { return _unloadCommand = _unloadCommand ?? new RelayCommand(Unload); }
        }

        public ICommand SetDefaultProjectCommand
        {
            get
            {
                return _setDefaultProjectCommand =
                    _setDefaultProjectCommand ?? new RelayCommand<ProjectItemViewModel>(SetDefaultProject);
            }
        }

        
        private void Load()
        {
            RegisterMessageHandlers();
        }


        private void Unload()
        {
            UnregisterMessageHandlers();
        }

        private void InitializeDefaultProject()
        {
            var defaultProject = LoadDefaultProject();

            if (defaultProject == null) return;

            var stageItem = StageItems.Single(stage => stage.Id.Equals(defaultProject.StageId));

            //SelectedStage = StageItems.Single(stage => stage.Id.Equals(defaultProject.StageId));

            DefaultProject =
                stageItem?.ProjectItems.Single(project => project.ProjectId.Equals(defaultProject.ProjectId));

            if (DefaultProject != null)
            {
                DefaultProject.IsSelected = true;

                stageItem.SelectedProject = DefaultProject;
            }
        }

        private bool CanStartProject()
        {
            if (StageTotal == 0)
                return false;

            return true;
            //return SelectedStage?.SelectedProject != _signInViewModel.SelectedProject;
        }

        private async void StartProject(ProjectItemViewModel projectItem)
        {
            var workingProject = _signInViewModel.SelectedProject;

            // 현재 작업중인 프로젝트 선택시
            //if (_signInViewModel.SelectedProject == SelectedStage?.SelectedProject)
            if (workingProject == projectItem)
            {
                IsProjectViewVisible = false;

                return;
            }

            // workingProject 에 값이 있다는건 프로젝트 변경을 의미
            if (workingProject != null)
            {
                var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();

                var dirtyTabs = subtitleViewModel?.Tabs.Where(tab => tab.CheckDirty()).ToList();

                var isDirty = dirtyTabs != null && dirtyTabs.Any();
                if (isDirty)
                {
                    var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(
                        Resource.CNT_WARNING,
                        "변경 내용이 있다~~",
                        MessageBoxButton.OKCancel,
                        Application.Current.MainWindow));

                    if (result == MessageBoxResult.Cancel)
                        return;
                }


                MessageCenter.Instance.Send(new ProjectSelect.ProjectChangeMessage(this));
            }

            // workingProject 값이 있다는 것은 프로젝트 재선택이라는 뜻
            if (workingProject != null)
                workingProject.IsSelected = false;

            if (DefaultProject != null)
                DefaultProject.IsSelected = false;

            projectItem.IsSelected = true;

            SelectedStage = StageItems.Single(stage => stage.Id.Equals(projectItem.StageId));

            _signInViewModel.SelectedStage = SelectedStage;

            _signInViewModel.SelectedProject = projectItem;

            IsProjectViewVisible = string.IsNullOrEmpty(_signInViewModel.SelectedProject?.ProjectId) ||
                                   string.IsNullOrEmpty(_signInViewModel.SelectedStage?.Id);

            await _languageLoader.LoadAsync();
        }

        private void OnStagePerPageNumberChanged(int obj)
        {
            StagePerPageNumber = obj;

            CalculateTotalPage();

            CalculateStageSlidePosition();
        }

        private void NavigateRight(string obj)
        {
            ++CurrentPageNumber;

            CalculateStageSlidePosition();
        }

        private void NavigateLeft(string obj)
        {
            if (CurrentPageNumber > 0)
                --CurrentPageNumber;

            CalculateStageSlidePosition();
        }

        public void Show()
        {
            IsProjectViewVisible = true;
            IsCancelButtonVisible = true;
            IsStartButtonVisible = true;
            CurrentPageNumber = 1;

            if (StageItems == null || _signInViewModel.SelectedProject == null)
                return;

            var stageItem =
                StageItems.SingleOrDefault(stage => stage.Id.Equals(_signInViewModel.SelectedProject.StageId));


            if (stageItem != null)
                stageItem.SelectedProject = _signInViewModel.SelectedProject;
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

        private void Close()
        {
            IsProjectViewVisible = false;
            IsCancelButtonVisible = false;
            IsStartButtonVisible = false;
            CurrentPageNumber = 1;
        }

        private void RegisterMessageHandlers()
        {
            if (_hasRegisteredMessageHandlers) return;
            _hasRegisteredMessageHandlers = true;
            MessageCenter.Instance.Regist<SignIn.LogoutMessage>(OnLogoutRequested);
            MessageCenter.Instance.Regist<SignIn.LoadStageProjectMessage>(LoadStageProject);
        }


        private void UnregisterMessageHandlers()
        {
            if (!_hasRegisteredMessageHandlers) return;
            MessageCenter.Instance.Unregist<SignIn.LogoutMessage>(OnLogoutRequested);
            MessageCenter.Instance.Unregist<SignIn.LoadStageProjectMessage>(LoadStageProject);
        }

        private void OnLogoutRequested(SignIn.LogoutMessage message)
        {
            IsProjectViewVisible = false;
            StageItems = null;
            CurrentPageStageItems = null;
            SelectedStage = null;

            MessageCenter.Instance.Send(new ProjectSelect.ProjectChangeMessage(this));
        }

        private async void LoadStageProject(SignIn.LoadStageProjectMessage message)
        {
            try
            {
                IsLoadingProjectPage = true;
                IsBusy = true;
                IsProjectViewVisible = true;

                var stages =
                    await _cloudMediaService.GetStagesAsync(new GetStagesParameter(_signInViewModel.GetAuthorizationAsync().Result),
                        CancellationToken.None);

                StageItems = stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                             new List<StageItemViewModel>();

                var emptyProjectStages = new List<StageItemViewModel>();

                foreach (var stageItem in StageItems)
                {
                    var projects = await _cloudMediaService.GetProjectsAsync(
                        new GetProjectsParameter(_signInViewModel.GetAuthorizationAsync().Result, stageItem.Id, stageItem.Name),
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

                foreach (var item in emptyProjectStages) StageItems.Remove(item);

                InitializeDefaultProject();
                //// Test Data 

                //var firstItem = StageItems.First();

                //var originalName = firstItem.Name;
                //for (var i = 1; i < 7; i++)
                //{
                //    var newItem = new StageItemViewModel(firstItem.Source)
                //    {
                //        Id = $"test{i}",
                //        Name = $"{originalName}_{i}"
                //        //ProjectItems = firstItem.ProjectItems.Select(item => new ProjectItemViewModel($"test{i}", item.Source)).ToList()
                //    };

                //    var projectModelList = new List<ProjectItemViewModel>();
                //    var count = 1;
                //    foreach (var firstItemProjectItem in firstItem.ProjectItems)
                //    {
                //        var project = new Project(
                //            $"{i}_{count}",
                //            firstItemProjectItem.Source.Name,
                //            firstItemProjectItem.Source.Description,
                //            firstItemProjectItem.Source.UsePlayout,
                //            firstItemProjectItem.Source.IsActive,
                //            firstItemProjectItem.Source.CreatedAt,
                //            firstItemProjectItem.Source.CreatedById,
                //            firstItemProjectItem.Source.CreatedByName,
                //            firstItemProjectItem.Source.CreatedByUsername,
                //            firstItemProjectItem.Source.UpdatedAt,
                //            firstItemProjectItem.Source.UpdatedById,
                //            firstItemProjectItem.Source.UpdatedByName,
                //            firstItemProjectItem.Source.UpdatedByUsername);

                //        projectModelList.Add(new ProjectItemViewModel(newItem.Id, project));
                //        count++;
                //    }

                //    newItem.ProjectItems = projectModelList;

                //    StageItems.Add(newItem);
                //}
                //// ----

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
                    _signInViewModel.Save();
                }

                IsBusy = false;
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }

        private void SetDefaultProject(ProjectItemViewModel projectItemViewModel)
        {
            if (projectItemViewModel == null) return;

            //원래 디폴트 프로젝트는 초기화
            if (DefaultProject == projectItemViewModel)
            {
                DefaultProject.IsDefault = !DefaultProject.IsDefault;
            }
            else
            {
                if (DefaultProject != null) DefaultProject.IsDefault = false;

                DefaultProject = projectItemViewModel;
                DefaultProject.IsDefault = true;
            }

            var item = new DefaultProjectSerialize
            {
                StageId = projectItemViewModel.StageId,
                ProjectId = projectItemViewModel.ProjectId
            };

            SaveDefaultProject(item);
        }

        private void SaveDefaultProject(DefaultProjectSerialize defaultProjectSerialize)
        {
            var profileData = JsonConvert.SerializeObject(defaultProjectSerialize);

            File.WriteAllText(DefaultProjectInfoFilePath, profileData);
        }

        private DefaultProjectSerialize LoadDefaultProject()
        {
            try
            {
                if (!File.Exists(DefaultProjectInfoFilePath))
                    return null;

                var profileData = File.ReadAllText(DefaultProjectInfoFilePath);
                return JsonConvert.DeserializeObject<DefaultProjectSerialize>(profileData);
            }
            catch (FileNotFoundException e)
            {
                _logger.Error.Write(e);
            }

            return null;
        }
    }
}