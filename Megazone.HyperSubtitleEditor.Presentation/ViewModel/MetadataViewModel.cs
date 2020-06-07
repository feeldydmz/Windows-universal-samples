using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    public class MetadataCaptionAssetInfo : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _id;

        public string Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public MetadataCaptionAssetInfo(string name, string id)
        {
            _name = name;
            _id = id;
        }
    }

    [Inject(Scope = LifetimeScope.Singleton)]
    public class MetadataViewModel : ViewModelBase
    {
        private static readonly string[] suffixes = {"bps", "Kbps", "Mbps", "Gbps", "Tbps", "Pbps"};

        private readonly WorkBarViewModel _workBarViewModel;
        private string _attributions;
        private string _categories;
        private ICommand _closeVideoInfoPopupCommand;
        private string _duration;
        private bool _isShow;
        private string _jobId;
        private string _videoId;
        private string _mediaType;
        private string _name;

        private ICommand _openMetadataPopupCommand;
        private List<MetadataCaptionAssetInfo> _captions;

        private string _status;
        private string _tags;
        
        internal MetadataViewModel(WorkBarViewModel workBarViewModel)
        {
            _workBarViewModel = workBarViewModel;

            _status = "None";
            _mediaType = "None";
            _duration = "None";
            _name = "None";
            _jobId = "None";
            _categories = "None";
            _attributions = "None";
            _tags = "None";
        }

        public bool IsShow
        {
            get => _isShow;
            set => Set(ref _isShow, value);
        }

        public ICommand OpenMetadataPopupCommand
        {
            get
            {
                return _openMetadataPopupCommand =
                    _openMetadataPopupCommand ?? new RelayCommand(Open);
            }
        }

        public ICommand CloseVideoInfoPopupCommand
        {
            get
            {
                return _closeVideoInfoPopupCommand =
                    _closeVideoInfoPopupCommand ?? new RelayCommand(Close);
            }
        }


        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public string MediaType
        {
            get => _mediaType;
            set => Set(ref _mediaType, value);
        }

        public string Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string VideoId
        {
            get => _videoId;
            set => Set(ref _videoId, value);
        }

        public string JobId
        {
            get => _jobId;
            set => Set(ref _jobId, value);
        }

        public string Categories
        {
            get => _categories;
            set => Set(ref _categories, value);
        }

        public string Attributions
        {
            get => _attributions;
            set => Set(ref _attributions, value);
        }

        public string Tags
        {
            get => _tags;
            set => Set(ref _tags, value);
        }

        public List<MetadataCaptionAssetInfo> Captions
        {
            get => _captions;
            set => Set(ref _captions, value);
        }

        public void Open()
        {
            Status = _workBarViewModel.VideoItem.Status ?? "None";

            MediaType = "";

            if (_workBarViewModel.VideoItem?.Source?.Sources != null)
                foreach (var renditionAsset in _workBarViewModel.VideoItem.Source.Sources)
                    if (MediaType.IsNullOrEmpty())
                        MediaType = $"{renditionAsset.Type}";
                    else
                        MediaType = $"{MediaType}, {renditionAsset.Type}";

            if (MediaType.IsNullOrEmpty())
                MediaType = "None";

            Duration = _workBarViewModel.VideoItem.Duration.ToString(@"hh\:mm\:ss");
            JobId = _workBarViewModel.VideoItem.Source?.Job?.Id ?? "None";
            VideoId = _workBarViewModel.VideoItem.Id?? "None";
            Name = _workBarViewModel.VideoItem.Name ?? "None";

            //if (_workBarViewModel?.VideoItem?.Source?.Sources != null)
            //    foreach (var renditionAsset in _workBarViewModel.VideoItem.Source.Sources)
            //    foreach (var renditionAssetElement in renditionAsset.Captions)
            //        if (renditionAssetElement.VideoSetting != null)
            //        {
            //            var videoSetting = renditionAssetElement.VideoSetting;
            //            var bitRateStr = ConvertBitRateToString(videoSetting.Bitrate);
            //            var rendition =
            //                $"{videoSetting.Codec}, {videoSetting.Width}x{videoSetting.Height}, {videoSetting.RatingControlMode}, {bitRateStr}";

            //            Captions.Add(rendition);
            //        }

            List<MetadataCaptionAssetInfo> captions= new List<MetadataCaptionAssetInfo>();

            if (_workBarViewModel?.VideoItem?.CaptionAssetList?.CaptionAssetItems != null)
            {
                foreach (var captionAssetItem in _workBarViewModel?.VideoItem?.CaptionAssetList?.CaptionAssetItems)
                {
                    //var caption = $"{captionAssetItem.Name} ({captionAssetItem.Id})";

                    captions.Add(new MetadataCaptionAssetInfo(captionAssetItem.Name, captionAssetItem.Id));
                }
            }

            if (!string.IsNullOrEmpty(_workBarViewModel?.CaptionAssetItem?.Id))
            {
                //var caption = $"{_workBarViewModel?.CaptionAssetItem.Name} ({_workBarViewModel?.CaptionAssetItem.Id})";

                var hasCaption =  captions.FirstOrDefault(c=>c.Id== _workBarViewModel.CaptionAssetItem.Id);

                if (hasCaption == null)
                    captions.Add(new MetadataCaptionAssetInfo(_workBarViewModel?.CaptionAssetItem.Name, _workBarViewModel?.CaptionAssetItem.Id));
            }

            Captions = captions;

            IsShow = true;
        }

        public void Close()
        {
            IsShow = false;
        }

        private string ConvertBitRateToString(double bitRate)
        {
            var counter = 0;

            while (Math.Round(bitRate / 1024) >= 1)
            {
                bitRate = bitRate / 1024;
                counter++;
            }

            bitRate = Math.Round(bitRate);

            return $"{bitRate}{suffixes[counter]}";
            ;
        }
    }
}