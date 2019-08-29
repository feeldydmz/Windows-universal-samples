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
    internal class McmDeployViewModel: ViewModelBase
    {
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly IBrowser _browser;
        private CaptionAssetItemViewModel _captionAssetItem;
        private bool _captionCreateMode;
        private IEnumerable<CaptionElementItemViewModel> _captionItems;
        private ICommand _confirmCommand;
        private ICommand _loadCommand;
        private string _projectName;
        private string _stageName;
        private VideoItemViewModel _videoItem;

        public McmDeployViewModel(SignInViewModel signInViewModel, SubtitleViewModel subtitleViewModel, IBrowser browser)
        {
            _signInViewModel = signInViewModel;
            _subtitleViewModel = subtitleViewModel;
            _browser = browser;
        }

        public Action CloseAction { get; set; }

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
            try
            {
                CaptionCreateMode = _subtitleViewModel.WorkContext.OpenedCaptionAsset == null;
                StageName = _signInViewModel.SelectedStage.Name;
                ProjectName = _signInViewModel.SelectedProject.Name;
                VideoItem = new VideoItemViewModel(_subtitleViewModel.WorkContext.OpenedVideo);
                CaptionAssetItem = _subtitleViewModel.WorkContext.OpenedCaptionAsset != null
                    ? new CaptionAssetItemViewModel(_subtitleViewModel.WorkContext.OpenedCaptionAsset)
                    : null;
                CaptionItems = _subtitleViewModel.WorkContext.Captions.Select(caption => new CaptionElementItemViewModel(caption)).ToList();
                foreach (var item in CaptionItems)
                    item.IsSelected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private bool CanConfirm()
        {
            return CaptionItems?.Where(x => x.IsSelected).Any() ?? false;
        }

        private void Confirm()
        {
            try
            {
                //var authorization = _signInViewModel.GetAuthorization();
                //var stageId = _signInViewModel.SelectedStage.Id;
                //var projectId = _signInViewModel.SelectedProject.ProjectId;
                var list = CaptionItems.Where(x => x.IsSelected).ToList();

                // upload caption files.
                //foreach (var caption in list)
                //{
                //    var uploadData = caption.Text;
                //    var fileName = caption.GetFileName();
                //    await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(authorization,
                //        stageId, projectId, uploadData, fileName, _uploadInputPath, caption.Url), CancellationToken.None);
                //}

                // update.
                CloseAction?.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
