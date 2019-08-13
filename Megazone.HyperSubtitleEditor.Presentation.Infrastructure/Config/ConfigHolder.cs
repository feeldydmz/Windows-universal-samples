using System;
using System.IO;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config
{
    public class ConfigHolder : ViewModelBase
    {
        private const string PROGRAM_KEY = "Megazone.HyperSubtitleEditor";

        private static ConfigHolder _current;
        private static ConfigHolder _default;

        private DownloadConfigHolder _download;
        private GeneralConfigHolder _general;
        private SubtitleConfigHolder _subtitle;
        private UploadConfigHolder _upload;

        public static BindableCurrentConfig BindableCurrent { get; } = new BindableCurrentConfig();

        public static ConfigHolder Current
        {
            get
            {
                if (_current == null)
                {
                    _current = Load();
                    BindableCurrent.Config = _current;
                }
                return _current;
            }
            private set
            {
                _current = value;
                BindableCurrent.Config = value;
            }
        }

        public static ConfigHolder Default
        {
            get { return _default = _default ?? LoadDefault(); }
        }

        private static string SavedDirectory => ObjectExtension.AppDataPath() + "\\";
        private static string SavedPath => SavedDirectory + "config.json";
        private static string DefaultConfigPath => SavedDirectory + "default_config.json";

        public DownloadConfigHolder Download
        {
            get => _download;
            set => Set(ref _download, value);
        }

        public GeneralConfigHolder General
        {
            get => _general;
            set => Set(ref _general, value);
        }

        public SubtitleConfigHolder Subtitle
        {
            get => _subtitle;
            set => Set(ref _subtitle, value);
        }

        public bool IsDirty => Subtitle.IsDirty;

        public UploadConfigHolder Upload
        {
            get => _upload;
            set => Set(ref _upload, value);
        }

        public static ConfigHolder Load()
        {
            try
            {
                var jsonString = File.ReadAllText(SavedPath);
                var jsonData = JsonConvert.DeserializeObject<RootJsonData>(jsonString);

                if (!jsonData.ProgramKey.Equals(PROGRAM_KEY))
                    throw new NotSupportedException();

                return new ConfigHolder
                {
                    General = new GeneralConfigHolder(jsonData.General),
                    Subtitle = new SubtitleConfigHolder(jsonData.Subtitle),
                    Download = new DownloadConfigHolder(jsonData.Download),
                    Upload = new UploadConfigHolder(jsonData.Upload)
                };
            }
            catch (Exception)
            {
                return LoadDefault();
            }
        }

        private static ConfigHolder LoadDefault()
        {
            try
            {
                var jsonString = File.ReadAllText(DefaultConfigPath);
                var jsonData = JsonConvert.DeserializeObject<RootJsonData>(jsonString);

                if (!jsonData.ProgramKey.Equals(PROGRAM_KEY))
                    throw new NotSupportedException();

                return new ConfigHolder
                {
                    General = new GeneralConfigHolder(jsonData.General),
                    Subtitle = new SubtitleConfigHolder(jsonData.Subtitle),
                    Download = new DownloadConfigHolder(jsonData.Download),
                    Upload = new UploadConfigHolder(jsonData.Upload)
                };
            }
            catch (Exception)
            {
                return new ConfigHolder
                {
                    General = new GeneralConfigHolder(new GeneralJsonData()),
                    Subtitle = new SubtitleConfigHolder(new SubtitleJsonData()),
                    Download = new DownloadConfigHolder(new DownloadJsonData()),
                    Upload = new UploadConfigHolder(new UploadJsonData())
                };
            }
        }

        public static void Save(ConfigHolder configHolder)
        {
            if (!Directory.Exists(SavedDirectory))
                Directory.CreateDirectory(SavedDirectory);
            File.WriteAllText(SavedPath, GetJsonString(configHolder));
            Current = configHolder;
        }

        public static bool Overwrite(string jsonString)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<RootJsonData>(jsonString);

                if (!jsonData.ProgramKey.Equals(PROGRAM_KEY))
                    return false;

                if (!Directory.Exists(SavedDirectory))
                    Directory.CreateDirectory(SavedDirectory);
                File.WriteAllText(SavedPath, jsonString);

                Current = new ConfigHolder
                {
                    General = new GeneralConfigHolder(jsonData.General),
                    Subtitle = new SubtitleConfigHolder(jsonData.Subtitle),
                    Download = new DownloadConfigHolder(jsonData.Download),
                    Upload = new UploadConfigHolder(jsonData.Upload)
                };

                return true;
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        public static string GetJsonString(ConfigHolder configHolder)
        {
            var jsonData = new RootJsonData
            {
                ProgramKey = PROGRAM_KEY,
                General = configHolder.General.GetJsonData(),
                Subtitle = configHolder.Subtitle.GetJsonData(),
                Download = configHolder.Download.GetJsonData(),
                Upload = configHolder.Upload.GetJsonData()
            };
            return JsonConvert.SerializeObject(jsonData);
        }

        public bool Equal(ConfigHolder config)
        {
            return General.Equal(config.General);
        }

        public class BindableCurrentConfig : ViewModelBase
        {
            private ConfigHolder _config;

            public ConfigHolder Config
            {
                get => _config;
                set => Set(ref _config, value);
            }
        }
    }
}