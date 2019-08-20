using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
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
        private readonly ICloudMediaService _cloudMediaService;

        private Authorization _authorization;

        private bool _canSignIn = true;

        private ICommand _enterPasswordCommand;
        private bool _isProjectViewVisible = true;
        private bool _isSignIn;
        private ICommand _loadedCommand;
        private ICommand _signInCommand;

        private string _loginId;

        private ICommand _logoutCommand;

        private ICommand _moveProjectStepCommand;
        private string _password;

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
        }

        public Action PasswordClearAction { get; set; }

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

        public bool CanSignIn
        {
            get => _canSignIn;
            set => Set(ref _canSignIn, value);
        }

        public bool IsSignIn
        {
            get => _isSignIn;
            set => Set(ref _isSignIn, value);
        }

        public string LoginId
        {
            get => _loginId;
            set => Set(ref _loginId, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand SignInCommand
        {
            get { return _signInCommand = _signInCommand ?? new RelayCommand(SigninAsync, () => CanSignIn); }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand = _logoutCommand ?? new RelayCommand(Logout); }
        }

        public ICommand MoveProjectStepCommand
        {
            get { return _moveProjectStepCommand = _moveProjectStepCommand ?? new RelayCommand(MoveProjectStep); }
        }

        public ICommand EnterPasswordCommand
        {
            get
            {
                return _enterPasswordCommand = _enterPasswordCommand ?? new RelayCommand<SecureString>(OnEnterPassword);
            }
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

        private void OnEnterPassword(SecureString secureString)
        {
            var password = secureString?.ReadString(true);
            Password = password;
            //Config.Proxy.ProxyDetail.Password = _password?.EncryptWithRfc2898(_keyGenerator.Generate());
        }

        private void OnLoaded()
        {
            ClearLoginInfo();

            // TODO: 자동 로그인.
            // 로컬에 저장된 Authorization 정보가 유효한지 확인한다.
        }

        private void ClearLoginInfo()
        {
            LoginId = string.Empty;
            Password = string.Empty;
            PasswordClearAction?.Invoke();
        }

        private async void SigninAsync()
        {
            IsProjectViewVisible = true;

            //if (string.IsNullOrEmpty(LoginId))
            //{
            //    MessageBox.Show("아이디를 입력하세요.");
            //    return;
            //}

            //if (string.IsNullOrEmpty(Password))
            //{
            //    MessageBox.Show("비밀번호를 입력하세요.");
            //    return;
            //}

            try
            {
                LoginId = "navan@mz.co.kr";
                Password = "jin!410c!!";

                CanSignIn = false;
                _authorization = await _cloudMediaService.LoginAsync(LoginId, Password);
                IsSignIn = !string.IsNullOrEmpty(_authorization?.AccessToken);

                // 로그인 실패.
                if (!IsSignIn)
                    return;
                
                var user = await _cloudMediaService.GetUserAsync(_authorization);

                StageItems = user?.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                             new List<StageItemViewModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                CanSignIn = true;
            }
        }

        private void Logout()
        {
            IsProjectViewVisible = true;
            IsSignIn = false;
            ClearLoginInfo();
            ClearAuthorization();
        }

        private void OnSelectProject(ProjectItemViewModel projectItemVm)
        {
            SelectedProject = projectItemVm;
            SelectedStage = StageItems.SingleOrDefault(stage => stage.Id.Equals(projectItemVm.StageId));

            if (!string.IsNullOrEmpty(SelectedProject?.ProjectId) && !string.IsNullOrEmpty(SelectedStage?.Id))
                IsProjectViewVisible = false;
        }
    }
}