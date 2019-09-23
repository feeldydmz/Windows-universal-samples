using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
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
        private const string _password = "Megazone@1";
        private readonly IBrowser _browser;
        private readonly ProjectViewModel _projectViewModel;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ConfigHolder _config;
        private readonly ILogger _logger;

        private Authorization _authorization;
        private bool _isAutoLogin;
        private bool _isBusy;
        private bool _isSignIn;
        private ICommand _loadCommand;
        private string _loginId;
        
        private ProjectItemViewModel _selectedProject;
        private StageItemViewModel _selectedStage;
        private string _uriSource;

        public SignInViewModel(ICloudMediaService cloudMediaService, ILogger logger, IBrowser browser, ProjectViewModel projectViewModel)
        {
            _logger = logger;
            _browser = browser;
            _projectViewModel = projectViewModel;
            _cloudMediaService = cloudMediaService;
            _config = ConfigHolder.Current;

            UriSource = "about:blank";
            _authorization = ReadSavedAuthorization();
        }

        internal string AuthorizationFilePath => $"{Path.GetTempPath()}subtitleAuthorization.json";
        public UserProfile User { get; set; }

        private string _username;
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

        public bool IsSignIn
        {
            get => _isSignIn;
            set => Set(ref _isSignIn, value);
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

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
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
            IsSignIn = false;
            SelectedProject = null;
            SelectedStage = null;

            _projectViewModel.IsProjectViewVisible = false;
            _projectViewModel.StageItems = null;
            _projectViewModel.CurrentPageStageItems = null;
            _projectViewModel.SelectingStage = null;
            
            UriSource = AuthorizationRepository.LOGIN_URL;
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
                    UriSource = AuthorizationRepository.LOGIN_URL;
                    return;
                }

                UserName = User.Name;
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

                    if (_config.Subtitle.AutoLogin)
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
                var profileData = JsonConvert.SerializeObject(_authorization).EncryptWithRfc2898(_password);

                File.WriteAllText(AuthorizationFilePath, profileData);
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }

        private Authorization ReadSavedAuthorization()
        {
            try
            {
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
                UriSource = AuthorizationRepository.LOGIN_URL;
                return;
            }

            if (_authorization != null)
            {
                LoadStageAndProjectAsync();
            }
            else
            {
                UriSource = AuthorizationRepository.LOGIN_URL;
                IsProjectViewVisible = false;
                IsSignIn = false;
            }
        }
        private ICommand _navigatingCommand;
        public ICommand NavigatingCommand
        {
            get { return _navigatingCommand = _navigatingCommand ?? new RelayCommand<string>(OnNavigating); }
        }

        private void OnNavigating(string code)
        {
            if (string.IsNullOrEmpty(code))
                return;

            LoginByAuthorizationCodeAsync(code);
        }
    }
}