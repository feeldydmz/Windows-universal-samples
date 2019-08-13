using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Megazone.Core.Log;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile.PersistantModel;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension;
using Newtonsoft.Json;
using Unity;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile
{
    public class ClientProfileManager
    {
        public const string PROGRAM_KEY = "Megazone.VideoStudio";
        private const string PROFILE_FOLDER_NAME = "Profiles";
        private const string PROFILE_FILE_NAME = "Profiles.json";
        private static readonly object _lockObject = new object();
        private readonly string _filePath;
        private readonly string _folderPath;
        private readonly ILogger _logger;
        private ClientProfiles _clientProfiles;

        private ClientProfileManager()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            var appDataPath = this.HyperTranscoderAppDataPath();
            _folderPath = appDataPath + PROFILE_FOLDER_NAME;
            _filePath = appDataPath + PROFILE_FOLDER_NAME + "\\" + PROFILE_FILE_NAME;
        }

        public static ClientProfileManager Instance { get; } = new ClientProfileManager();

        public string LoginProfileId { get; set; }

        public string SelectedProfileId => _clientProfiles?.SelectedProfileId;

        public ClientProfile GetProfile(string profileId)
        {
            return _clientProfiles?.Profiles?.FirstOrDefault(s => s.Id == profileId);
        }

        public ClientProfile GetSelectedProfile()
        {
            return _clientProfiles?.Profiles?.FirstOrDefault(s => s.Id == _clientProfiles?.SelectedProfileId);
        }

        public ClientProfiles Load()
        {
            if (_clientProfiles == null)
                lock (_lockObject)
                {
                    try
                    {
                        if (_clientProfiles == null)
                            if (File.Exists(_filePath))
                            {
                                var jsonString = File.ReadAllText(_filePath);
                                if (!string.IsNullOrEmpty(jsonString))
                                {
                                    var convertedModel = JsonConvert.DeserializeObject<ClientProfilesModel>(jsonString);
                                    _clientProfiles = new ClientProfiles(convertedModel);
                                }
                            }
                    }
                    catch
                    {
                        // ignored 
                    }
                }
            return _clientProfiles ?? (_clientProfiles = new ClientProfiles(PROGRAM_KEY));
        }

        public void Save()
        {
            if (_clientProfiles == null)
                return;
            MergeProfileRegions();
            lock (_lockObject)
            {
                try
                {
                    var jsonString = JsonConvert.SerializeObject(_clientProfiles.ToPersistentModel(true));
                    if (!Directory.Exists(_folderPath))
                        Directory.CreateDirectory(_folderPath);
                    File.WriteAllText(_filePath, jsonString);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void MergeProfileRegions()
        {
            if (!RegionManager.Instance.IsReady)
                return;
            if (_clientProfiles.ProfileRegions == null)
                _clientProfiles.ProfileRegions = new List<ProfileRegion>();
            var newList = new List<ProfileRegion>(_clientProfiles.ProfileRegions);
            var existingProfileRegion = newList.FirstOrDefault(n => n.ProfileID == SelectedProfileId);
            if (existingProfileRegion != null)
                newList.Remove(existingProfileRegion);
            var selectedProfileRegion = new ProfileRegion(SelectedProfileId)
            {
                RegionInformations = RegionManager.Instance.Regions?.ToList() ?? new List<RegionInformation>()
            };
            newList.Add(selectedProfileRegion);
            _clientProfiles.ProfileRegions = newList;
        }

        public string GetJsonString()
        {
            if (_clientProfiles == null)
                Load();

            _clientProfiles.ProgramKey = PROGRAM_KEY;

            return JsonConvert.SerializeObject(_clientProfiles);
        }

        public bool Merge(string jsonString)
        {
            try
            {
                var clientProfilesData = JsonConvert.DeserializeObject<ClientProfilesModel>(jsonString);
                if (clientProfilesData != null)
                {
                    if (!clientProfilesData.ProgramKey?.Equals(PROGRAM_KEY) ?? false)
                        return false;
                    var importedClientProfiles = new ClientProfiles(clientProfilesData);
                    _clientProfiles.SelectedProfileId = importedClientProfiles.SelectedProfileId;
                    var importedProfiles = importedClientProfiles.Profiles?.ToList();
                    var importedProfileRegions =
                        importedClientProfiles.ProfileRegions?.ToList() ?? new List<ProfileRegion>();
                    if (importedProfiles == null) return false;
                    var existingClientProfiles = Load();
                    _clientProfiles.Profiles =
                        MergeProfiles(existingClientProfiles, clientProfilesData, importedProfiles);
                    _clientProfiles.ProfileRegions =
                        MergeProfileRegions(existingClientProfiles.ProfileRegions?.ToList(), importedProfileRegions);

                    if (_clientProfiles.Profiles.All(p => p.Id != _clientProfiles.SelectedProfileId))
                        _clientProfiles.SelectedProfileId = null;
                    Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
            return false;
        }

        private IEnumerable<ProfileRegion> MergeProfileRegions(List<ProfileRegion> existingProfileRegions,
            List<ProfileRegion> importedProfileRegions)
        {
            if (importedProfileRegions == null || !importedProfileRegions.Any()) return new List<ProfileRegion>();
            if (existingProfileRegions == null || !existingProfileRegions.Any())
                return new List<ProfileRegion>(importedProfileRegions);
            foreach (var importedProfileRegion in importedProfileRegions)
            {
                var sameProfileRegion =
                    existingProfileRegions.FirstOrDefault(s => s.ProfileID == importedProfileRegion.ProfileID);
                if (sameProfileRegion != null)
                    sameProfileRegion.RegionInformations =
                        new List<RegionInformation>(importedProfileRegion.RegionInformations);
                else
                    existingProfileRegions.Add(new ProfileRegion(importedProfileRegion.ProfileID)
                    {
                        RegionInformations = new List<RegionInformation>(importedProfileRegion.RegionInformations)
                    });
            }
            return existingProfileRegions;
        }

        private IEnumerable<ClientProfile> MergeProfiles(ClientProfiles existingClientProfiles,
            ClientProfilesModel clientProfilesData,
            List<ClientProfile> importedProfiles)
        {
            var existingProfiles = existingClientProfiles.Profiles?.ToList();
            if (existingProfiles == null || !existingProfiles.Any())
                return clientProfilesData.Profiles?.Select(p => new ClientProfile(p))
                    .ToList();
            foreach (var importedProfile in importedProfiles)
            {
                var sameProfile = existingProfiles.FirstOrDefault(s => s.Id == importedProfile.Id);
                if (sameProfile != null)
                {
                    sameProfile.Name = importedProfile.Name;
                    sameProfile.ClientProfileCredentialInfo = importedProfile.ClientProfileCredentialInfo?.Clone();
                    sameProfile.LastAccessedDateTimeTicks =
                        sameProfile.LastAccessedDateTimeTicks < importedProfile.LastAccessedDateTimeTicks
                            ? importedProfile.LastAccessedDateTimeTicks
                            : sameProfile.LastAccessedDateTimeTicks;
                    sameProfile.RememberedPipelineIdForAutoConnection =
                        importedProfile.RememberedPipelineIdForAutoConnection;
                }
                else
                {
                    existingProfiles.Add(importedProfile);
                }
            }
            return existingProfiles;
        }
    }
}