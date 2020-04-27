﻿using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    // 이건 삭제해야되..
    internal class McmWorkContext
    {
        public McmWorkContext(Video openedVideo = null, CaptionAsset openedCaptionAsset = null)
        {
            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            VideoResolutionsByType = GetVideoUrlDictionary(openedVideo);
            VideoUrlOfResolutions = VideoResolutionsByType?.FirstOrDefault().Value;
            if (VideoUrlOfResolutions?.Count == 0)
                VideoMediaUrl = VideoResolutionsByType?.FirstOrDefault().Key.Url;
            else
                VideoMediaUrl = VideoUrlOfResolutions?.FirstOrDefault().Value ?? "";
        }

        private Video OpenedVideo { get; set; }
        private CaptionAsset OpenedCaptionAsset { get; set; }

        public string VideoMediaUrl { get; private set; }
        public Dictionary<VideoResolutionInfo, string> VideoUrlOfResolutions { get; private set; }

        public Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>> VideoResolutionsByType
        {
            get;
            private set;
        }

        public void Initialize(Video openedVideo, CaptionAsset openedCaptionAsset)
        {
            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            VideoResolutionsByType = GetVideoUrlDictionary(openedVideo);
            VideoUrlOfResolutions = VideoResolutionsByType?.FirstOrDefault().Value;
            VideoMediaUrl = VideoUrlOfResolutions?.FirstOrDefault().Value ?? "";
        }

        private Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>> GetVideoUrlDictionary(Video video)
        {
            if (video?.Sources == null) return null;

            var resultDictionary = new Dictionary<MediaKind, Dictionary<VideoResolutionInfo, string>>();

            //해상도별 URL
            foreach (var renditionAsset in video.Sources)
            {
                if (renditionAsset.Elements == null) continue;

                var resolutionDictionary = new Dictionary<VideoResolutionInfo, string>();

                var orderingList = renditionAsset.Elements.OrderByDescending(item => item.VideoSetting?.Height);
                foreach (var element in orderingList)
                {
                    if (element.VideoSetting == null) continue;

                    var url = element.AccessUrls?.FirstOrDefault() ?? renditionAsset.AccessUrls?.FirstOrDefault() ?? "";
                    resolutionDictionary.Add(
                        new VideoResolutionInfo(element.VideoSetting.Width, element.VideoSetting.Height,
                            element.VideoSetting.Codec),
                        url);
                }

                // renditionAsset Url이 비어있다면 resolutionDictionary의 첫번째 url로 채워준다
                var baseUrl = renditionAsset.AccessUrls?.FirstOrDefault() ??
                              resolutionDictionary.Values.FirstOrDefault() ?? "";
                resultDictionary.Add(new MediaKind(renditionAsset.Type, baseUrl), resolutionDictionary);
            }

            return resultDictionary;
        }

        private string GetVideoMediaUrl(Video video)
        {
            // video영상을 가져온다.
            var asset = video?.Sources?.FirstOrDefault(rendition => rendition.Type.ToUpper().Equals("HLS")) ??
                        video?.Sources?.FirstOrDefault();

            if (asset == null)
                return string.Empty;

            var url = asset.AccessUrls?.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
                url = asset.Elements?.FirstOrDefault()?.AccessUrls?.FirstOrDefault();
            return url;
        }
    }
}