using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Strategy
{
    internal class DefaultSettingSaveStrategy : ISettingSaveStrategy
    {
        public void Save(ConfigHolder config, bool isSilence = false)
        {
            config = CreateConfigClone(config);
            if (isSilence)
            {
                ConfigHolder.Save(config);
                return;
            }

            ConfigHolder.Save(config);
        }

        private ConfigHolder CreateConfigClone(ConfigHolder config)
        {
            return new ConfigHolder
            {
                Subtitle = new SubtitleConfigHolder(config.Subtitle.GetJsonData()),
                Upload = new UploadConfigHolder(config.Upload.GetJsonData()),
                Download = new DownloadConfigHolder(config.Download.GetJsonData()),
                General = new GeneralConfigHolder(config.General.GetJsonData())
            };
        }
    }
}