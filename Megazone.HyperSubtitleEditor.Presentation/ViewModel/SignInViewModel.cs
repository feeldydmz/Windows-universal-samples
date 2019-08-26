using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Repository;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Security.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Newtonsoft.Json;
using Unity;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class SignInViewModel : ViewModelBase
    {
        private const string AUTHORIZATION_ENDPOINT = "https://megaone.io";
        internal string ProfilePath { get; set; }

        private readonly ICloudMediaService _cloudMediaService;
        private Authorization _authorization;

        private string _uriSource;
        private bool _isProjectViewVisible = true;
        private bool _isSignIn;

        private ICommand _loadedCommand;
        private ICommand _navigatingCommand;
        private ICommand _moveProjectStepCommand;

        private ProjectItemViewModel _selectedProject;
        private StageItemViewModel _selectedStage;
        private List<StageItemViewModel> _stageItems;

        //PjectSelectView 전용
        private bool _isLeftNavigateButtonVisible = false;
        private bool _isRightNavigateButtonVisible = false;
        private bool _isNotExistContentVisible = false;
        private bool _isCancleButtonVisible = false;

        private int _stageTotal;
        private int _slideNavigateBarPosition = 0;
        private IEnumerable<StageItemViewModel> _currentPageStageItems;

        private ICommand _startProjectCommand;
        private ICommand _closeWindowCommand;
        private ICommand _rightNavigateCommand;
        private ICommand _leftSlideNavigateCommand;
        private ILogger _logger;


        public Authorization GetAuthorization()
        {
            // 유효성 검사.
            // 유효한 토큰인지 확인한다.
            // 유효하지 않다면, refresh token을 받도록 exception을 낸다.
            return _authorization;
        }

        public SignInViewModel(ICloudMediaService cloudMediaService)
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();

            _cloudMediaService = cloudMediaService;

            UriSource = "https://megaone.io/oauth/authorize?response_type=code&client_id=0a31e7dc-65eb-4430-9025-24f9e3d7d60d&redirect_uri=https://console.media.megazone.io/megaone/login";

            ProfilePath = $"{this.AppDataPath()}\\profile.json";
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

        public StageItemViewModel SelectedStage
        {
            get => _selectedStage;
            set => Set(ref _selectedStage, value);
        }

        public ProjectItemViewModel SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != null)
                    _selectedProject.IsSelected = false;

                Set(ref _selectedProject, value);

                if (_selectedProject != null)
                    _selectedProject.IsSelected = true;
            } 
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

        public bool IsNotExistContentVisible
        {
            get => _isNotExistContentVisible;
            set => Set(ref _isNotExistContentVisible, value);
        }

        public bool IsCancleButtonVisible
        {
            get => _isCancleButtonVisible;
            set => Set(ref _isCancleButtonVisible, value);
        }

        public string UriSource
        {
            get => _uriSource;
            set => Set(ref _uriSource, value);
        }

        public int StageTotal
        {
            get => _stageTotal;
            set => Set(ref _stageTotal, value);
        }
        public int NavigateBarPosition
        {
            get => _slideNavigateBarPosition;
            set => Set(ref _slideNavigateBarPosition, value);
        }

        public ICommand MoveProjectStepCommand
        {
            get { return _moveProjectStepCommand = _moveProjectStepCommand ?? new RelayCommand(MoveProjectStep); }
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        

        public ICommand NavigatingCommand
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(OnNavigating); }
        }

        public ICommand LeftSlideNavigateCommand
        {
            get { return _leftSlideNavigateCommand = _leftSlideNavigateCommand ?? new RelayCommand<string>(OnLeftSlideNavigate); }
        }
        public ICommand RightSlideNavigateCommand
        {
            get { return _rightNavigateCommand = _rightNavigateCommand ?? new RelayCommand<string>(OnRightSlideNavigate); }
        }

        private void OnRightSlideNavigate(string obj)
        {
            ++_slideNavigateBarPosition;

            CalculateStageSlidePosition();
        }

        private void OnLeftSlideNavigate(string obj)
        {
            if (_slideNavigateBarPosition > 0)
                --_slideNavigateBarPosition;

            CalculateStageSlidePosition();
        }

        public ICommand StartProjectCommand
        {
            get
            {
                return _startProjectCommand =
                    _startProjectCommand ?? new RelayCommand(OnStartProject, CanStartProject);
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand =
                    _closeWindowCommand ?? new RelayCommand(OnCloseWindow);
            }
        }

        private bool CanStartProject()
        {
            // TODO 일단 연결 가능한 스테이지나 프로젝트가 없어도 화면으로 넘어가게 
            if (StageTotal == 0)
                return true;

            return SelectedProject != null;
        }

        private void MoveProjectStep()
        {
            IsProjectViewVisible = true;
        }

        private void ClearAuthorization()
        {
            _authorization = null;
            // 저장된 Authorization 정보를 삭제한다.
        }

        private void CalculateStageSlidePosition()
        {
            var startIndex = NavigateBarPosition * 3;
            var endIndex = startIndex + 3;

            var newStageList = new List<StageItemViewModel>();
            for (var i = startIndex; i < endIndex; i++)
            {
                if (i >= StageItems.Count)
                    break;

                newStageList.Add(StageItems[i]);
            }

            CurrentPageStageItems = newStageList;

            if (NavigateBarPosition == 0 && StageTotal > 3)
            {
                IsRightNavigateButtonVisible = true;
                IsLeftNavigateButtonVisible = false;
            }
            else if (NavigateBarPosition > 0 && 
                     (NavigateBarPosition == (int)(StageTotal / NavigateBarPosition - 1)))
            {
                IsRightNavigateButtonVisible = false;
                IsLeftNavigateButtonVisible = true;
            }
            else if (NavigateBarPosition > 0 && StageTotal > 3)
            {
                IsRightNavigateButtonVisible = true;
                IsLeftNavigateButtonVisible = true;
            }
            else
            {
                IsRightNavigateButtonVisible = false;
                IsLeftNavigateButtonVisible = false;
            }
        }
       
        private void Logout()
        {
            IsProjectViewVisible = false;
            IsSignIn = false;
            ClearAuthorization();
        }

        private void LoginByAuthorizationCode(string code)
        {
            var authorizationRepository = new AuthorizationRepository();

            var authResponse = authorizationRepository.Get(new AuthorizationRequest(AUTHORIZATION_ENDPOINT, code));

            try
            {
                if (authResponse.AccessToken.IsNullOrEmpty()) return;

                _authorization = new Authorization(authResponse.AccessToken, null, null);

                LoadStageAndProject();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void LoadStageAndProject()
        {
            IsProjectViewVisible = true;
            IsSignIn = true;

            var user = await _cloudMediaService.GetUserAsync(_authorization, CancellationToken.None);

            StageItems = user?.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                         new List<StageItemViewModel>();

            //// --- Test Data 

            //StageItemViewModel firstItem = StageItems.First();

            //string originalName = firstItem.Name;
            //for (int i = 1; i < 7; i++)
            //{
            //    StageItemViewModel newItem = new StageItemViewModel(firstItem)
            //    {
            //        Id = "D",
            //        Name = $"{originalName}_{i}"
            //    };
            //    StageItems.Add(newItem);
            //}
            //// ----

            StageTotal = StageItems.Count();

            if (StageTotal == 0)
            {
                IsNotExistContentVisible = true;
            }
            else
            {
                CalculateStageSlidePosition();
            }
        }

        private void SaveProfile()
        {
            var profileData = JsonConvert.SerializeObject(_authorization).EncryptWithRfc2898("Megazone@1");

            File.WriteAllText(ProfilePath, profileData);
        }

        private bool LoadProfile()
        {
            try
            {
                string profileData = File.ReadAllText(ProfilePath);

                _authorization =
                    JsonConvert.DeserializeObject<Authorization>(profileData.DecryptWithRfc2898("Megazone@1"));

                return (_authorization != null && !_authorization.AccessToken.IsNullOrEmpty());
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);

                return false;
            }
        }

        private void OnLoaded()
        {
            //TODO 로그인 체크 여부 셋팅이 추가되면 되면 주석 제거
            //if (LoadProfile())
            //{
            //    LoadStageAndProject();
            //}
        }

        private void OnStartProject()
        {
            Console.WriteLine(SelectedProject.Name);

            SelectedStage = StageItems.SingleOrDefault(stage => stage.Id.Equals(SelectedProject.StageId));

            if (!string.IsNullOrEmpty(SelectedProject?.ProjectId) && !string.IsNullOrEmpty(SelectedStage?.Id))
            {
                SaveProfile();
                IsProjectViewVisible = false;
            }
        }

        private void OnCloseWindow()
        {
            IsProjectViewVisible = false;
        }

        private void OnNavigating(string code)
        {
            if (code.IsNullOrEmpty()) return;

            Task.Run(() => LoginByAuthorizationCode(code));
        }
    }
}