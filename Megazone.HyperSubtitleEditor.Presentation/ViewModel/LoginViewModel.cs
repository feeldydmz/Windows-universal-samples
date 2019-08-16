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
    public class LoginViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;

        private Authorization _authorization;

        private ICommand _enterPasswordCommand;
        private bool _isLogin;
        private bool _isProjectViewVisible = true;
        private ICommand _loadedCommand;
        private ICommand _loginCommand;

        private string _loginId;

        private ICommand _logoutCommand;

        private ICommand _moveProjectStepCommand;
        private string _password;

        private ProjectItemViewModel _selectedProject;

        private StageItemViewModel _selectedStage;
        private ICommand _selectProjectCommand;
        private IEnumerable<StageItemViewModel> _stageItems;

        public LoginViewModel(ICloudMediaService cloudMediaService)
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

        public bool IsLogin
        {
            get => _isLogin;
            set => Set(ref _isLogin, value);
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

        public ICommand LoginCommand
        {
            get { return _loginCommand = _loginCommand ?? new RelayCommand(LoginAsync); }
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

        private void Logout()
        {
            IsProjectViewVisible = true;
            IsLogin = false;
            ClearLoginInfo();
            ClearAuthorization();
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

        private async void LoginAsync()
        {
            IsProjectViewVisible = true;

            if (string.IsNullOrEmpty(LoginId))
            {
                MessageBox.Show("아이디를 입력하세요.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }

            try
            {
                _authorization = await _cloudMediaService.LoginAsync(LoginId, Password);
                IsLogin = !string.IsNullOrEmpty(_authorization?.AccessToken);

                var user = await _cloudMediaService.GetUserAsync(_authorization);
                var stageItemList = user?.Stages?.Select(stage => new StageItemViewModel(stage)).ToList() ??
                                    new List<StageItemViewModel>();
                StageItems = stageItemList;
                //StageItems = new ObservableCollection<StageItemViewModel>(stageItemList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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