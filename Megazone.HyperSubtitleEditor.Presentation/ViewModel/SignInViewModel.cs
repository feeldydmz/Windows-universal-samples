using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Repository;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Security.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class SignInViewModel : ViewModelBase
    {
        private const string AUTHORIZATION_ENDPOINT = "https://megaone.io";

        private readonly ICloudMediaService _cloudMediaService;

        private Authorization _authorization;

        private string _uriSource;
        //private ICommand _enterPasswordCommand;
        //private ICommand _signInCommand;
        //private string _loginId;
        //private ICommand _logoutCommand;

        private bool _isProjectViewVisible = true;
        private bool _isSignIn;

        private ICommand _navigatingCommand;
        private ICommand _moveProjectStepCommand;
        //private string _password;

        private ProjectItemViewModel _selectedProject;

        private StageItemViewModel _selectedStage;
        private ICommand _selectProjectCommand;
        private IEnumerable<StageItemViewModel> _stageItems;
        

        public Authorization GetAuthorization()
        {
            // 유효성 검사.
            // 유효한 토큰인지 확인한다.
            // 유효하지 않다면, refresh token을 받도록 exception을 낸다.
            return _authorization;
        }

        public SignInViewModel(ICloudMediaService cloudMediaService)
        {
            _cloudMediaService = cloudMediaService;

            UriSource = "https://megaone.io/oauth/authorize?response_type=code&client_id=0a31e7dc-65eb-4430-9025-24f9e3d7d60d&redirect_uri=https://console.media.megazone.io/megaone/login";
        }

        public IEnumerable<StageItemViewModel> StageItems
        {
            get => _stageItems;
            set => Set(ref _stageItems, value);
        }

        public StageItemViewModel SelectedStage
        {
            get => _selectedStage;
            set => Set(ref _selectedStage, value);
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

        public string UriSource
        {
            get => _uriSource;
            set => Set(ref _uriSource, value);
        }

        public ICommand MoveProjectStepCommand
        {
            get { return _moveProjectStepCommand = _moveProjectStepCommand ?? new RelayCommand(MoveProjectStep); }
        }

        public ICommand NavigatingCommand
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(OnNavigating); }
        }

        public ICommand SelectProjectCommand
        {
            get
            {
                return _selectProjectCommand =
                    _selectProjectCommand ?? new RelayCommand<ProjectItemViewModel>(OnSelectProject);
            }
        }

        private void MoveProjectStep()
        {
            IsProjectViewVisible = true;
        }

        private void ClearAuthorization()
        {
            // 저장된 Authorization 정보를 삭제한다.
        }

        //private async void SigninAsync()
        //{
        //    IsProjectViewVisible = true;

        //    //if (string.IsNullOrEmpty(LoginId))
        //    //{
        //    //    MessageBox.Show("아이디를 입력하세요.");
        //    //    return;
        //    //}

        //    //if (string.IsNullOrEmpty(Password))
        //    //{
        //    //    MessageBox.Show("비밀번호를 입력하세요.");
        //    //    return;
        //    //}

        //    try
        //    {
        //        //LoginId = "navan@mz.co.kr";
        //        //Password = "jin!410c!!";

        //        CanSignIn = false;
        //        //_authorization = await _cloudMediaService.LoginAsync(LoginId, Password);
        //        IsSignIn = !string.IsNullOrEmpty(_authorization?.AccessToken);

        //        // 로그인 실패.
        //        if (!IsSignIn)
        //            return;
                
        //        var user = await _cloudMediaService.GetUserAsync(_authorization);

        //        StageItems = user?.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
        //                     new List<StageItemViewModel>();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    finally
        //    {
        //        CanSignIn = true;
        //    }
        //}

        private void Logout()
        {
            IsProjectViewVisible = true;
            IsSignIn = false;
            ClearAuthorization();
        }

        private async void LoginByAuthorizationCode(string code)
        {
            var authorizationRepository = new AuthorizationRepository();

            var authResponse = authorizationRepository.Get(new AuthorizationRequest(AUTHORIZATION_ENDPOINT, code));

            try
            {
                if (authResponse.AccessToken.IsNullOrEmpty()) return;

                _authorization = new Authorization(authResponse.AccessToken, null, null);
                IsProjectViewVisible = true;
                IsSignIn = true;

                var user = await _cloudMediaService.GetUserAsync(_authorization);

                StageItems = user?.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                             new List<StageItemViewModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnSelectProject(ProjectItemViewModel projectItemVm)
        {
            SelectedProject = projectItemVm;
            SelectedStage = StageItems.SingleOrDefault(stage => stage.Id.Equals(projectItemVm.StageId));

            if (!string.IsNullOrEmpty(SelectedProject?.ProjectId) && !string.IsNullOrEmpty(SelectedStage?.Id))
                IsProjectViewVisible = false;
        }

        private void OnNavigating(string code)
        {
            if (code.IsNullOrEmpty()) return;

            Task.Run(() => LoginByAuthorizationCode(code));
        }
    }
}