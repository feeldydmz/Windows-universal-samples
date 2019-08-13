using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Storage.ServiceInterface.S3;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Enum;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
using Megazone.Core.Extension;
using Megazone.Core.Reference;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class JobListItemOutputViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly PresetLoader _presetLoader;
        private readonly IS3Service _s3Service;
        private ICommand _copyLinkCommand;
        private MediaType _displayMediaType = MediaType.Unknown;
        private string _displayName;
        private string _displayPresetName;
        private bool _isChecked;

        public JobListItemOutputViewModel()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _presetLoader = Bootstrapper.Container.Resolve<PresetLoader>();
            _s3Service = Bootstrapper.Container.Resolve<IS3Service>();
        }

        public string OutputKeyPrefix { get; internal set; }
        public IList<string> OutputKeys { get; set; } = new List<string>();
        public IList<string> PresetIds { get; set; } = new List<string>();
        public string RelativePath { get; set; }
        public string Extension { get; internal set; }
        public string FullUrl { get; internal set; }
        public Status OutputStatus { get; internal set; }

        public MediaType DisplayMediaType
        {
            get => _displayMediaType;
            set => Set(ref _displayMediaType, value);
        }

        public string DisplayName
        {
            get => _displayName;
            set => Set(ref _displayName, value);
        }

        public string DisplayPresetNames
        {
            get
            {
                if (string.IsNullOrEmpty(_displayPresetName))
                    return InitializePresetNames();
                return _displayPresetName;
            }
            set => Set(ref _displayPresetName, value);
        }

        public ICommand CopyLinkCommand
        {
            get { return _copyLinkCommand = _copyLinkCommand ?? new RelayCommand(CopyLink, CanCopyLink); }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => Set(ref _isChecked, value);
        }

        private string InitializePresetNames()
        {
            if (!_presetLoader.IsPresetsAvailable)
            {
                _presetLoader.Load(new WeakAction<IList<Preset>>(SetPresetNamesHandler));
                return null;
            }
            return SetPresetNames(_presetLoader.Presets);
        }

        private string SetPresetNames(IList<Preset> presets)
        {
            var temp = string.Empty;
            foreach (var presetId in PresetIds)
            {
                var foundPreset = presets.FirstOrDefault(p => p.Id == presetId);
                if (foundPreset == null) continue;
                var replacedName = foundPreset.Name.Replace("System preset:", "")
                    .TrimStart();
                temp += replacedName + ", ";
            }
            temp = temp.TrimEnd();
            if (temp.IsNotNullOrAny())
                temp = temp.Substring(0, temp.Length - 1);

            return temp;
        }

        private void SetPresetNamesHandler(IList<Preset> presets)
        {
            this.InvokeOnUi(() => { DisplayPresetNames = SetPresetNames(presets); });
        }

        private bool CanCopyLink()
        {
            // TODO : 재 확인.
            return true;
        }

        private void CopyLink()
        {
            Clipboard.SetText(FullUrl);
            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                string.Format(Resource.MSG_PATH_COPY_TO_CLIPBOARD, DisplayPresetNames.Trim()), MessageBoxButton.OK));
        }

        internal void Initialize()
        {
            var url = RelativePath.Contains(Extension)
                ? $"{OutputKeyPrefix}{RelativePath}"
                : $"{OutputKeyPrefix}{RelativePath}.{Extension}";
            var baseUrl = _s3Service.GetUrlWith(AppContext.CredentialInfo.Region,
                PipelineLoader.Instance.SelectedPipeline.OutputBucket);
            FullUrl = $"{baseUrl}{url}";
        }
    }
}