using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class McmDeployViewModel: ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private CaptionAssetItemViewModel _captionAssetItem;
        private bool _captionCreateMode;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private ICommand _confirmCommand;
        private bool _isBusy;
        private ICommand _loadCommand;
        private string _projectName;
        private string _stageName;
        private string _uploadInputPath;
        private VideoItemViewModel _videoItem;

        public McmDeployViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel,
            IBrowser browser)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
            _browser = browser;
        }

        public Action CloseAction { get; set; }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(LoadAsync); }
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

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        private async void LoadAsync()
        {
            IsBusy = true;
            try
            {
                CaptionCreateMode = WorkContext.Caption == null;
                StageName = _signInViewModel.SelectedStage.Name;
                ProjectName = _signInViewModel.SelectedProject.Name;
                VideoItem = new VideoItemViewModel(WorkContext.Video);
                CaptionAssetItem = WorkContext.Caption != null
                    ? new CaptionAssetItemViewModel(WorkContext.Caption)
                    : null;
                CaptionItems = WorkContext.Captions.Select(caption => new CaptionElementItemViewModel(caption))
                    .ToList();
                foreach (var item in CaptionItems)
                    item.IsSelected = true;

                _uploadInputPath = await GetUploadInputPathAsync();
                if (string.IsNullOrEmpty(_uploadInputPath))
                {
                    // 메시지 처리.
                    // 게시할수 없음.
                    _browser.ShowConfirmWindow(
                        new ConfirmWindowParameter("오류", "MCM의 업로드 설정 정보가 없습니다.", MessageBoxButton.OK));
                    CloseAction?.Invoke();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanConfirm()
        {
            return CaptionItems?.Where(x => x.IsSelected).Any() ?? false;
        }

        private async void Confirm()
        {
            Debug.Assert(!string.IsNullOrEmpty(_uploadInputPath), "_uploadInputPath is empty.");

            IsBusy = true;
            try
            {
                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;
                var list = CaptionItems.Where(x => x.IsSelected).ToList();

                // upload caption files.
                foreach (var caption in list)
                {
                    var uploadData = caption.Text;
                    var fileName = caption.GetFileName();
                    await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(authorization,
                        stageId, projectId, uploadData, fileName, _uploadInputPath, caption.Url), CancellationToken.None);
                }

                // update.
                CloseAction?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<string> GetUploadInputPathAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var setting =
                await _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId), CancellationToken.None);

            var uploadTargetPath = string.Empty;
            if (setting != null)
            {
                var s3Path = setting.Asset?.InputStoragePrefix?.Value;
                var folderPath = setting.Asset?.InputStoragePath?.Value;
                if (!string.IsNullOrEmpty(s3Path))
                    uploadTargetPath = $"{s3Path}{folderPath}";

                if (string.IsNullOrEmpty(uploadTargetPath))
                {
                    s3Path = setting.General?.StoragePrefix?.Value;
                    folderPath = setting.General?.StoragePath?.Value;
                    if (!string.IsNullOrEmpty(s3Path))
                        uploadTargetPath = $"{s3Path}{folderPath}";
                }
            }

            if (!string.IsNullOrEmpty(uploadTargetPath))
            {
                var uri = new Uri(uploadTargetPath);
                uploadTargetPath = $"{uri.Host}{uri.LocalPath}";
            }

            return uploadTargetPath;
        }
    }
}
