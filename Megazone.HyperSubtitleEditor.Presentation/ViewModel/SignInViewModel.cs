using System;
using System.IO;
using System.Threading;
using System.Windows.Input;
using Megazone.Cloud.Media.Repository;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Security.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Newtonsoft.Json;


namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class SignInViewModel : ViewModelBase
    {
        private const string _password = "Megazone@1";
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ConfigHolder _config;
        private readonly ILogger _logger;

        private Authorization _authorization;
        private bool _isAutoLogin;
        private bool _isBusy;
        private ICommand _loadCommand;
        private string _loginId;
        private ICommand _navigatingCommand;

        private ProjectItemViewModel _selectedProject;
        private StageItemViewModel _selectedStage;
        private string _uriSource;
        private string _username;

        public Action<string> OnSourceUriChanged;

        public SignInViewModel(ICloudMediaService cloudMediaService, ILogger logger)
        {
            _logger = logger;
            _cloudMediaService = cloudMediaService;
            _config = ConfigHolder.Current;

            UriSource = "about:blank";
            _authorization = LoadAuthorization();
        }

        internal string AuthorizationFilePath => $"{Path.GetTempPath()}subtitleAuthorization.dat";

        public string UserName
        {
            get => _username;
            set => Set(ref _username, value);
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

        public bool IsAutoLogin
        {
            get => _isAutoLogin;
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

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public Action CloseAction { get; set; }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand NavigatingCommand
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(NavigateToProject); }
        }

        public ICommand Command
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(NavigateToProject); }
        }

        private void SetSourceUri(string url)
        {
            UriSource = url;
            OnSourceUriChanged?.Invoke(url);
        }

        public void Save()
        {
            _config.Subtitle.AutoLogin = IsAutoLogin;
            ConfigHolder.Save(_config);

            if (_config.Subtitle.AutoLogin)
                SaveAuthorization();
        }

        public Authorization GetAuthorization()
        {
            // 유효성 검사.
            // 유효한 토큰인지 확인한다.
            // 유효하지 않다면, refresh token을 받도록 exception을 낸다.
            return _authorization;
        }

        public void Logout()
        {
            SelectedProject = null;
            SelectedStage = null;

            //프로젝트 선택 화면 초기화
            MessageCenter.Instance.Send(new SignIn.LogoutMessage(this));
            //view 로그인 화면 생성
            MessageCenter.Instance.Send(new SignIn.CreateSignInViewMessage(this));

            //SetSourceUri(AuthorizationRepository.LOGIN_URL);
        }

        private async void LoginByAuthorizationCodeAsync(string code)
        {
            try
            {
                _authorization = await _cloudMediaService.LoginByAuthorizationCodeAsync(code, CancellationToken.None);

                if (string.IsNullOrEmpty(_authorization?.AccessToken))
                    return;

                OpenProjectViewAsync();
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
        }

        private async void OpenProjectViewAsync()
        {
            try
            {
                IsBusy = true;
                var user = await _cloudMediaService.GetUserAsync(_authorization, CancellationToken.None);

                // 유저 인증 실패 401
                if (user == null)
                {
                    IsBusy = false;
                    SetSourceUri(AuthorizationRepository.LOGIN_URL);
                    return;
                }

                UserName = user.Name;
                LoginId = user.Username;

                MessageCenter.Instance.Send(new SignIn.LoadStageProjectMessage(this, user));

                CloseAction?.Invoke();
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SaveAuthorization()
        {
            try
            {
                var profileData = JsonConvert.SerializeObject(_authorization).EncryptWithRfc2898(_password);

                File.WriteAllText(AuthorizationFilePath, profileData);
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }

        private Authorization LoadAuthorization()
        {
            try
            {
                if (!File.Exists(AuthorizationFilePath))
                    return null;

                var profileData = File.ReadAllText(AuthorizationFilePath);
                return JsonConvert.DeserializeObject<Authorization>(profileData.DecryptWithRfc2898(_password));
            }
            catch (FileNotFoundException e)
            {
                _logger.Error.Write(e);
            }

            return null;
        }

        public void ClearAuthorization()
        {
            _authorization = null;

            if (File.Exists(AuthorizationFilePath))
                File.Delete(AuthorizationFilePath);
        }

        private void Load()
        {
            IsAutoLogin = _config.Subtitle.AutoLogin;
            if (!_config.Subtitle.AutoLogin)
            {
                SetSourceUri(AuthorizationRepository.LOGIN_URL);
                return;
            }

            if (_authorization != null)
            {
                OpenProjectViewAsync();
            }
            else
            {
                SetSourceUri(AuthorizationRepository.LOGIN_URL);
            }
        }

        public void NavigateToProject(string code)
        {
            if (string.IsNullOrEmpty(code))
                return;

            SetSourceUri("about:blank");

            LoginByAuthorizationCodeAsync(code);
        }
    }
}