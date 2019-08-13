using System;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Strategy;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class SettingViewModel : ViewModelBase
    {
        private RelayCommand _applyCommand;
        private RelayCommand _applySubtitleCommand;
        private ConfigHolder _config;
        private RelayCommand _initiateCommand;
        private RelayCommand _initiateSubtitleCommand;
        private RelayCommand _saveCommand;

        public SettingViewModel()
        {
            Config = CreateCurrentConfigClone();
        }

        public ConfigHolder Config
        {
            get => _config;
            set => Set(ref _config, value);
        }

        public RelayCommand InitiateSubtitleCommand
        {
            get
            {
                return
                    _initiateSubtitleCommand =
                        _initiateSubtitleCommand ??
                        new RelayCommand(InitiateSubtitle, () => !ConfigHolder.Default.Subtitle.Equal(Config.Subtitle));
            }
        }

        public RelayCommand ApplySubtitleCommand
        {
            get
            {
                return
                    _applySubtitleCommand =
                        _applySubtitleCommand ?? new RelayCommand(ApplySubtitle, () => Config.Subtitle.IsDirty);
            }
        }

        public RelayCommand InitiateCommand
        {
            get
            {
                return
                    _initiateCommand =
                        _initiateCommand ?? new RelayCommand(Initiate, () => !ConfigHolder.Default.Equal(Config));
            }
        }

        public RelayCommand ApplyCommand
        {
            get { return _applyCommand = _applyCommand ?? new RelayCommand(Apply, () => Config.IsDirty); }
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand = _saveCommand ?? new RelayCommand(Save); }
        }

        public Action CloseAction { get; set; }

        private void Apply()
        {
            Save(Config);
        }

        private void ApplySubtitle()
        {
            var config = CreateCurrentConfigClone();
            config.Subtitle = Config.Subtitle;
            Save(config);
        }

        private ConfigHolder CreateCurrentConfigClone()
        {
            return new ConfigHolder
            {
                Subtitle = new SubtitleConfigHolder(ConfigHolder.Current.Subtitle.GetJsonData()),
                Upload = new UploadConfigHolder(ConfigHolder.Current.Upload.GetJsonData()),
                Download = new DownloadConfigHolder(ConfigHolder.Current.Download.GetJsonData()),
                General = new GeneralConfigHolder(ConfigHolder.Current.General.GetJsonData())
            };
        }

        private void Save(ConfigHolder config)
        {
            SettingSaveStrategyFactory.GetDefault()
                .Save(config);
            MessageCenter.Instance.Send(new Subtitle.SettingsSavedMessage(this));
        }

        private void Save()
        {
            Save(Config);

            CloseAction?.Invoke();
        }

        private void Initiate()
        {
            InitiateSubtitle();
        }

        private void InitiateSubtitle()
        {
            Config.Subtitle = new SubtitleConfigHolder(ConfigHolder.Default.Subtitle.GetJsonData());
        }
    }
}