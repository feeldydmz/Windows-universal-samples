namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal static class PlaylistFormatExtension
    {
        public static string ToDisplayValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            switch (value.ToLower())
            {
                case "hlsv3":
                    return "HLS v3";
                case "hlsv4":
                    return "HLS v4";
                case "mpeg-dash":
                    return "MPEG-D";
                case "smooth":
                    return "Smooth";
            }

            return null;
        }

        public static string GetExtension(this string format)
        {
            if (string.IsNullOrEmpty(format))
                return null;
            switch (format.ToLower())
            {
                case "hlsv3":
                case "hlsv4":
                    return "m3u8";
                case "mpeg-dash":
                    return "mpd";
                case "smooth":
                    return "ism";
            }

            return null;
        }

        public static string GetPresetContainer(this string format)
        {
            if (string.IsNullOrEmpty(format))
                return null;
            switch (format.ToLower())
            {
                case "hlsv3":
                case "hlsv4":
                    return "ts";
                case "mpeg-dash":
                case "smooth":
                    return "fmp4";
            }

            return null;
        }
    }
}