using System;
using Megazone.Cloud.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal static class MediaTypeExtension
    {
        public static string ToDisplayValue(this MediaType valuea)
        {
            switch (valuea)
            {
                case MediaType.Unknown:
                    return "Unknown";
                case MediaType.AdaptiveStreaming:
                    return "Adaptive Streaming";
                case MediaType.DownloadableVideo:
                    return "Downloadable Video";
                case MediaType.DownloadableAudio:
                    return "Downloadable Audio";
                default:
                    throw new ArgumentOutOfRangeException(nameof(valuea), valuea, null);
            }
        }

        public static MediaType ToMediaType(this string container)
        {
            if (string.IsNullOrEmpty(container)) return MediaType.Unknown;
            switch (container.ToUpper())
            {
                case "TS":
                case "FMP4":
                    return MediaType.AdaptiveStreaming;
                case "MP4":
                case "WEBM":
                case "MXF":
                case "FLV":
                case "GIF":
                case "MPG":
                    return MediaType.DownloadableVideo;
                case "MP3":
                case "FLAC":
                case "WAV":
                case "OGA":
                case "OGG:":
                    return MediaType.DownloadableAudio;
                default:
                    return MediaType.Unknown;
            }
        }
    }
}