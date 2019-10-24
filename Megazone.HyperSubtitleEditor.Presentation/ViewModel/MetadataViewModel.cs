using System;
using System.Collections.Generic;
using System.Windows.Input;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class MetadataViewModel : ViewModelBase
    {
        private static readonly string[] suffixes = {"bps", "Kbps", "Mbps", "Gbps", "Tbps", "Pbps"};
        private string _attributions;
        private string _categories;
        private ICommand _closeVideoInfoPopupCommand;
        private string _duration;
        private bool _isOpenVideoInfoPopup;
        private string _jobId;
        private string _mediaType;
        private string _name;

        private ICommand _openVideoInfoPopupCommand;
        private List<string> _renditions;

        private string _status;
        private string _tags;

        private readonly WorkBarViewModel _workBarViewModel;

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

            Renditions = new List<string>();
        }

        public bool IsOpenVideoInfoPopup
        {
            get => _isOpenVideoInfoPopup;
            set => Set(ref _isOpenVideoInfoPopup, value);
        }

        public ICommand OpenVideoInfoPopupCommand
        {
            get
            {
                return _openVideoInfoPopupCommand =
                    _openVideoInfoPopupCommand ?? new RelayCommand(Open);
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

        public List<string> Renditions
        {
            get => _renditions;
            set => Set(ref _renditions, value);
        }

        public void Open()
        {
            Status = _workBarViewModel.VideoItem.Status ?? "None";

            MediaType = "";

            Renditions?.Clear();

            if (_workBarViewModel.VideoItem?.Source?.Sources != null)
                foreach (var renditionAsset in _workBarViewModel.VideoItem.Source.Sources)
                    if (MediaType.IsNullOrEmpty())
                        MediaType = $"{renditionAsset.Type}";
                    else
                        MediaType = $"{MediaType}, {renditionAsset.Type}";

            if (MediaType.IsNullOrEmpty())
                MediaType = "None";

            Duration = _workBarViewModel.VideoItem.Duration.ToString(@"hh\:mm\:ss");
            JobId = _workBarViewModel.VideoItem.Id ?? "None";
            Name = _workBarViewModel.VideoItem.Name ?? "None";

            if (_workBarViewModel?.VideoItem?.Source?.Sources != null)
                foreach (var renditionAsset in _workBarViewModel.VideoItem.Source.Sources)
                foreach (var renditionAssetElement in renditionAsset.Elements)
                    if (renditionAssetElement.VideoSetting != null)
                    {
                        var videoSetting = renditionAssetElement.VideoSetting;
                        var bitRateStr = ConvertBitRateToString(videoSetting.Bitrate);
                        var rendition =
                            $"{videoSetting.Codec}, {videoSetting.Width}x{videoSetting.Height}, {videoSetting.RatingControlMode}, {bitRateStr}";

                        Renditions.Add(rendition);
                    }

            IsOpenVideoInfoPopup = true;
        }

        public void Close()
        {
            IsOpenVideoInfoPopup = false;
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