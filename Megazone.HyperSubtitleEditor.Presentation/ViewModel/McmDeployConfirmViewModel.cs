using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using System.Threading.Tasks;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class McmDeployConfirmViewModel: ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private ICommand _loadCommand;
        private ICommand _confirmCommand;
        private string _stageName;
        private string _projectName;
        private VideoItemViewModel _videoItem;
        private CaptionAssetItemViewModel _captionAssetItem;
        private bool _captionCreateMode;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;

        public Action CloseAction { get; set; }

        public McmDeployConfirmViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }
        
        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }
        
        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
        }

        public string StageName
        {
            get => _stageName;
            set => Set(ref _stageName, value);
        }

        public string ProjectName
        {
            get => _projectName;
            set => Set(ref _projectName, value);
        }

        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }
        public CaptionAssetItemViewModel CaptionAssetItem
        {
            get => _captionAssetItem;
            set => Set(ref _captionAssetItem, value);
        }

        public IEnumerable<CaptionElementItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }
        public bool CaptionCreateMode
        {
            get => _captionCreateMode;
            set => Set(ref _captionCreateMode, value);
        }

        private void Load()
        {
            if(WorkContext.Video == null)
            {
                throw new Exception("");
            }

            CaptionCreateMode = WorkContext.Caption == null;
            StageName = _signInViewModel.SelectedStage.Name;
            ProjectName = _signInViewModel.SelectedProject.Name;
            VideoItem = new VideoItemViewModel(WorkContext.Video);
            CaptionAssetItem = WorkContext.Caption != null ? new CaptionAssetItemViewModel(WorkContext.Caption) : null;
            CaptionItems = WorkContext.Captions.Select(caption=> new CaptionElementItemViewModel(caption)).ToList();
        }

        private bool CanConfirm()
        {
            return CaptionItems?.Where(x => x.IsSelected).Any() ?? false;
        }

        private async void Confirm()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var list = CaptionItems.Where(x => x.IsSelected).Select(x => x.Source).ToList();

            var uploadPath = GetUploadPathAsync();

            // upload caption files.
            foreach (var caption in list)
            {
                var uploadUrl = caption.Url;
                if (string.IsNullOrEmpty(uploadUrl))
                {
                    uploadUrl = $"{uploadPath}/{caption.Label}_{caption.Language}.vtt";
                }

                var uploadData = string.Empty;
                await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(uploadUrl, uploadData));
            }

            // update.
            CloseAction?.Invoke();


            
        }

        async Task<string> GetUploadPathAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var setting = await _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId));

            var uploadPath = string.Empty;
            if (setting != null)
            {
                var s3Path = setting.Asset?.InputStoragePrefix?.Value;
                var folderPath = setting.Asset?.InputStoragePath?.Value;
                if (!string.IsNullOrEmpty(s3Path))
                    uploadPath = $"{s3Path}{folderPath}";
            }
            if (string.IsNullOrEmpty(uploadPath))
            {
                var s3Path = setting.General?.StoragePrefix?.Value;
                var folderPath = setting.General?.StoragePath?.Value;
                if (!string.IsNullOrEmpty(s3Path))
                    uploadPath = $"{s3Path}{folderPath}";
            }
            return uploadPath;
        }
    }
}
