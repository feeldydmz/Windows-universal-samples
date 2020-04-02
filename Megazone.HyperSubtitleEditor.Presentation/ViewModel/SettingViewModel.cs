using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Strategy;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class SettingViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitle;

        private DisplayItem _languageSelectedItem;

        private RelayCommand _applyCommand;
        private ConfigHolder _config;
        private RelayCommand _initiateCommand;
        private RelayCommand _initiateSubtitleCommand;
        private RelayCommand _confirmCommand;

        public SettingViewModel()
        {
            Config = CreateCurrentConfigClone();

            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitle = Bootstrapper.Container.Resolve<SubtitleViewModel>();

            LanguageSelectedItem = LanguageOptions.Single(x => x.Key == Config.Subtitle.Language);
        }

        public ConfigHolder Config
        {
            get => _config;
            set => Set(ref _config, value);
        }

        private IEnumerable<DisplayItem> _languageOptions = new List<DisplayItem>()
        {
            new DisplayItem("자동", ""),
            new DisplayItem("한국어", "ko-KR"),
            new DisplayItem("English", "en-US")
        };

        public IEnumerable<DisplayItem> LanguageOptions
        {
            get => _languageOptions;
            set => Set(ref _languageOptions, value);
        }

        public DisplayItem LanguageSelectedItem
        {
            get => _languageSelectedItem;
            set
            {
                Set(ref _languageSelectedItem, value);

                Config.Subtitle.Language = _languageSelectedItem.Key;
            }
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
            get { return _applyCommand = _applyCommand ?? new RelayCommand(Apply, () => Config.Subtitle.IsDirty); }
        }

        public RelayCommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm); }
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

        internal void AskForRestart()
        {
            var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_SETTING,
                Resource.MSG_CHANGE_LANGUAGE,
                MessageBoxButton.OKCancel,
                Application.Current.MainWindow));

            if (result == MessageBoxResult.Cancel)
                return;

            _browser.Main.RestartMainWindow();

//            var applicationPath = this.StartUpPath() + this.GetApplicationName();
//
//            // 강제 종료시키고, 재실행하도록 한다.
//            if (Application.Current.MainWindow != null)
//                Application.Current.MainWindow.Close();
//            Process.Start(applicationPath, "-r");
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
            bool isChangedLanguage = !Config.Subtitle.Language.Equals(ConfigHolder.Current.Subtitle.Language);

            SettingSaveStrategyFactory.GetDefault()
                .Save(config);

            if (isChangedLanguage)
                AskForRestart();
            
            MessageCenter.Instance.Send(new Message.SubtitleEditor.SettingsSavedMessage(this));
        }

        private void Confirm()
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

            LanguageSelectedItem = LanguageOptions.Single(x => x.Key == Config.Subtitle.Language);
        }
    }
}