using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class ProjectViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly SignInViewModel _signInViewModel;
        private ICommand _closeCommand;

        private int _currentPageNumber;

        private IEnumerable<StageItemViewModel> _currentPageStageItems;
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

        private ICommand _rightNavigateCommand;
        private StageItemViewModel _selectingStage;
        private bool _selectionsChangedFlag;

        private ICommand _selectProjectCommand;
        private List<StageItemViewModel> _stageItems;
        private ICommand _stagePerPageNumberChangedCommand;
        private int _stageTotal;

        private ICommand _startProjectCommand;
        private int _totalPage;


        public ProjectViewModel(IBrowser browser,SignInViewModel signInViewModel)
        {
            _browser = browser;
            _signInViewModel = signInViewModel;
            CurrentPageNumber = 1;
        }

        public bool IsProjectViewVisible
        {
            get => _isProjectViewVisible;
            set => Set(ref _isProjectViewVisible, value);
        }

        public StageItemViewModel SelectingStage
        {
            get => _selectingStage;
            set => Set(ref _selectingStage, value);
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

        public ICommand StartProjectCommand
        {
            get
            {
                return _startProjectCommand =
                    _startProjectCommand ?? new RelayCommand(StartProject, CanStartProject);
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

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        private bool CanStartProject()
        {
            if (StageTotal == 0)
                return false;

            return SelectingStage?.SelectedProject != _signInViewModel.SelectedProject;
        }

        private void StartProject()
        {
            Console.WriteLine($@"StagePerPageNumber : {StagePerPageNumber}");

            if (_signInViewModel.SelectedProject != null)
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

            _signInViewModel.SelectedStage = SelectingStage;
            _signInViewModel.SelectedProject = SelectingStage.SelectedProject;
            IsProjectViewVisible = string.IsNullOrEmpty(_signInViewModel.SelectedProject?.ProjectId) ||
                                   string.IsNullOrEmpty(_signInViewModel.SelectedStage?.Id);
        }

        private void SelectProject(ProjectItemViewModel projectItem)
        {
            if (!_selectionsChangedFlag)
            {
                if (StageItems == null) return;

                _selectionsChangedFlag = true;

                SelectingStage = StageItems.Single(stage => stage.Id.Equals(projectItem.StageId));

                foreach (var stage in StageItems)
                    if (!stage.Equals(SelectingStage))
                        if (stage.SelectedProject != null)
                            stage.SelectedProject = null;
                _selectionsChangedFlag = false;
            }
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

        public void Show()
        {
            IsProjectViewVisible = true;
            IsCancelButtonVisible = true;
            IsStartButtonVisible = true;
            CurrentPageNumber = 1;

            if (StageItems == null || _signInViewModel.SelectedProject == null)
                return;

            var stageItem = StageItems.SingleOrDefault(stage => stage.Id.Equals(_signInViewModel.SelectedProject.StageId));
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
    }
}