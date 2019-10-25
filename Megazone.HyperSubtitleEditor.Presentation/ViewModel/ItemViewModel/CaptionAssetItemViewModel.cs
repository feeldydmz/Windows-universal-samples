using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    /// <summary>
    ///     Caption asset view model.
    /// </summary>
    internal class CaptionAssetItemViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private IEnumerable<CaptionElementItemViewModel> _elements;
        private string _kind;

        private string _name;

        private CaptionAssetItemViewModel()
        {
            
            Name = Resource.CNT_MAKE_NEW_CAPTION;
        }

        public CaptionAssetItemViewModel(CaptionAsset asset)
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

        public CaptionAsset Source { get; }
        public string Id { get; }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

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

        public void Initialize()
        {
            Elements?.ToList().ForEach(element => element.IsSelected = false);
        }

        public void SelectAll()
        {
            if (Elements != null)
                foreach (var element in Elements)
                    element.IsSelected = true;
        }

        public static CaptionAssetItemViewModel Empty => new CaptionAssetItemViewModel();
        
    }
}