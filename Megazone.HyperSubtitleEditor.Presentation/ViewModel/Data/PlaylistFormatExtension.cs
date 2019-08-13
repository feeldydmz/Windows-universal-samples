using System;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal static class PlaylistFormatExtension
    {
        public static string GetExtension(this PlaylistFormat format)
        {
            switch (format)
            {
                case PlaylistFormat.HlsV3:
                case PlaylistFormat.HlsV4:
                    return "m3u8";
                case PlaylistFormat.MpegDash:
                    return "mpd";
                case PlaylistFormat.Smooth:
                    return "ism";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static string ToDisplayValue(this PlaylistFormat value)
        {
            switch (value)
            {
                case PlaylistFormat.None:
                    return null;
                case PlaylistFormat.HlsV3:
                    return "HLS v3";
                case PlaylistFormat.HlsV4:
                    return "HLS v4";
                case PlaylistFormat.MpegDash:
                    return "MPEG-Dash";
                case PlaylistFormat.Smooth:
                    return "Smooth";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public static PresetContainer GetPresetContainer(this PlaylistFormat format)
        {
            switch (format)
            {
                case PlaylistFormat.HlsV3:
                    return PresetContainer.Ts;
                case PlaylistFormat.HlsV4:
                    return PresetContainer.Ts;
                case PlaylistFormat.MpegDash:
                    return PresetContainer.Fmp4;
                case PlaylistFormat.Smooth:
                    return PresetContainer.Fmp4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}