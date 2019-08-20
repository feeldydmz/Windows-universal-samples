using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    /// <summary>
    ///  Caption asset view model.
    /// </summary>
    public class CaptionAssetItemViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private ICommand _loadCommand;
        private IEnumerable<CaptionElementItemViewModel> _elements;
        private string _kind;

        public CaptionAssetItemViewModel(Asset<Caption> asset)
        {
            _cloudMediaService = Bootstrapper.Container.Resolve<ICloudMediaService>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();

            Source = asset;
            Id = asset.Id;
            Name = asset.Name;
            IsActive = asset.Status?.ToLower().Equals("active") ?? false;
            Type = asset.Type;
            MediaType = asset.MediaType;
            Duration = asset.Duration;
            Version = asset.Version;
            CreatedAt = string.IsNullOrEmpty(asset.CreatedAt)
                ? DateTime.MinValue
                : DateTimeOffset.Parse(asset.CreatedAt).DateTime;
            Elements = asset.Elements?.Select(element => new CaptionElementItemViewModel(element)).ToList();
            Kind = asset.Elements?.FirstOrDefault()?.Kind;
        }

        public Asset<Caption> Source { get; }
        public string Id { get; }
        public string Name { get; }
        public bool IsActive { get; }
        public string Type { get; }
        public string MediaType { get; }
        public long Duration { get; }
        public int Version { get; }
        public DateTime CreatedAt { get; }

        public IEnumerable<CaptionElementItemViewModel> Elements
        {
            get => _elements;
            set => Set(ref _elements, value);
        }

        public string Kind
        {
            get => _kind;
            set => Set(ref _kind, value);
        }
        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(LoadAsync); }
        }
        private void LoadAsync()
        {
            if (Elements != null)
                return;

            //var authorization = _signInViewModel.GetAuthorization();
            //var stageId = _signInViewModel.SelectedStage?.Id;
            //var projectId = _signInViewModel.SelectedStage?.Id;

            //var caption = await _cloudMediaService.GetCaptionAsync(new GetCaptionParameter(authorization, stageId, projectId, Id));
            //Elements = caption.Elements?.Select(element => new CaptionElementItemViewModel(element)).ToList();
        }
    }
}
