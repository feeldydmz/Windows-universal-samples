using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal static class FileTypeDistributer
    {
        public static readonly IList<string> SupportedVideoExtensions = new List<string>
        {
            ".avi",
            ".mkv",
            ".wmv",
            ".mpg",
            ".mpeg",
            ".divx",
            ".mp4",
            ".asf",
            ".flv",
            ".mov",
            ".m4v",
            ".vob",
            ".ogv",
            ".webm",
            ".ts",
            ".m2ts",
            ".avs",
            ".mxf",
            ".drc",
            ".gifv",
            ".mng",
            ".qt",
            ".yuv",
            ".rm",
            ".rmvb",
            ".amv",
            ".m4p",
            ".mpe",
            ".mp2",
            ".mpv",
            ".m2v",
            ".svi",
            ".3gp",
            ".3g2",
            ".roq",
            ".nsv",
            ".f4v",
            ".f4p",
            ".f4a",
            ".f4b"
        };

        public static readonly IList<string> SupportedAudioExtensions = new List<string>
        {
            ".aac",
            ".mp3",
            ".mp2",
            ".pcm",
            ".flac",
            ".ogg",
            ".oga",
            ".wav"
        };

        public static bool IsVideoFormat(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".avi":
                case ".mkv":
                case ".wmv":
                case ".mpg":
                case ".mpeg":
                case ".divx":
                case ".mp4":
                case ".asf":
                case ".flv":
                case ".mov":
                case ".m4v":
                case ".vob":
                case ".ogv":
                case ".webm":
                case ".ts":
                case ".m2ts":
                case ".avs":
                case ".mxf":
                case ".drc":
                case ".gifv":
                case ".mng":
                case ".qt":
                case ".yuv":
                case ".rm":
                case ".rmvb":
                case ".amv":
                case ".m4p":
                case ".mpe":
                case ".mp2":
                case ".mpv":
                case ".m2v":
                case ".svi":
                case ".3gp":
                case ".3g2":
                case ".roq":
                case ".nsv":
                case ".f4v":
                case ".f4p":
                case ".f4a":
                case ".f4b":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAudioFormat(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".aac":
                case ".mp3":
                case ".mp2":
                case ".pcm":
                case ".flac":
                case ".ogg":
                case ".oga":
                case ".wav":
                    //case ".wma": 
                    //case ".mpa":
                    //case ".m4a":
                    //case ".ape":
                    //case ".aiff":
                    //case ".mka":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSubtitleFormat(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".dfxp":
                case ".scc":
                case ".srt":
                case ".ttml":
                case ".vtt":
                case ".stl":
                case ".xml":
                    //case ".wsrt":
                    //case ".txt":
                    //case ".ass":
                    //case ".aqt":
                    //case ".stp":
                    //case ".detx":
                    //case ".cac":
                    //case ".crk":
                    //case ".csv":
                    //case ".sub":
                    //case ".edl":
                    //case ".rtf":
                    //case ".fcpxml":
                    //case ".ttxt":
                    //case ".gst":
                    //case ".ats":
                    //case ".itt":
                    //case ".json":
                    //case ".lrc":
                    //case ".imtpro":
                    //case ".mpl":
                    //case ".flc":
                    //case ".pjs":
                    //case ".caption":
                    //case ".rt":
                    //case ".smi":
                    //case ".html":
                    //case ".ssa":
                    //case ".sif":
                    //case ".tek":
                    //case ".tt":
                    //case ".subtitle":
                    //case ".xsubtitle":
                    //case ".trs":
                    //case ".tmx":
                    //case ".tts":
                    //case ".usf":
                    //case ".utx":
                    //case ".xif":
                    //case ".sbv":
                    //case ".zeg":
                    //case ".titl":
                    //case ".cip":
                    //case ".C":
                    //case ".asc":
                    //case ".pst":
                    //case ".cap":
                    //case ".pac":
                    //case ".890":
                    //case ".spt":
                    //case ".WSB":
                    //case ".chk":
                    //case ".cin":
                    //case ".ult":
                    //case ".elr":
                    //case ".uld":
                    //case ".sst":
                    //case ".mks":
                    //case ".sup":
                    //case ".dost":
                    //case ".aya":
                    //case ".fpc":
                    //case ".son":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsImageFormat(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".bmp":
                case ".dib":
                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".jfif":
                case ".tif":
                case ".tiff":
                case ".gif":
                case ".png":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSupportedSubtitleFormat(string fileExtension)
        {
            return fileExtension == ".vtt";
        }
    }
}