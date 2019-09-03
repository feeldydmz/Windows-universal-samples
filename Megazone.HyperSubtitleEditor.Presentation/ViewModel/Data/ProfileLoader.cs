using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension;
using Newtonsoft.Json;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class ProfileLoader
    {
        private const string PROFILE_FOLDER_NAME = "Profiles";
        private const string PROFILE_FILE_NAME = "Profiles.json";
        private readonly string _filePath;
        private readonly ILogger _logger;

        private ClientProfiles _clientProfiles;

        public ProfileLoader(IUnityContainer unityContainer)
        {
            _logger = unityContainer.Resolve<ILogger>();
            var appDataPath = this.HyperTranscoderAppDataPath();
            _filePath = appDataPath + PROFILE_FOLDER_NAME + "\\" + PROFILE_FILE_NAME;
        }

        public ClientProfile GetProfile(string profileId)
        {
            return _clientProfiles?.Profiles?.FirstOrDefault(s => s.Id == profileId);
        }

        public ClientProfiles Load()
        {
            if (_clientProfiles != null) return _clientProfiles;
            try
            {
                if (File.Exists(_filePath))
                {
                    var jsonString = File.ReadAllText(_filePath);
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var convertedModel = JsonConvert.DeserializeObject<ClientProfilesModel>(jsonString);
                        _clientProfiles = new ClientProfiles(convertedModel);
                    }
                }
                else
                {
                    _clientProfiles = new ClientProfiles(new List<ClientProfile>());
                }
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
                _clientProfiles = new ClientProfiles(new List<ClientProfile>());
            }

            return _clientProfiles;
        }

        public string GetJsonString()
        {
            if (_clientProfiles == null)
                Load();
            _clientProfiles.ProgramKey = ClientProfileManager.PROGRAM_KEY;
            return JsonConvert.SerializeObject(_clientProfiles);
        }
    }
}