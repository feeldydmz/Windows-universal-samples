using System;
using System.IO;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension
{
    public static class ObjectExtension
    {
        public static string HyperTranscoderAppDataPath(this object obj)
        {
            return HyperTranscoderAppDataPath();
        }

        public static string HyperSubtitleEditorAppDataPath(this object obj)
        {
            return HyperSubtitleEditorAppDataPath();
        }

        public static string HyperTranscoderAppDataPath()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Megazone\Hyper Media Transcoder\";
        }

        public static string HyperSubtitleEditorAppDataPath()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Megazone\Hyper Subtitle Editor\";
        }

        public static string MyDocuments(this object obj)
        {
            return MyDocuments();
        }

        public static string MyDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static string DownloadFolder()
        {
            var pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(pathUser, "Downloads");
        }

        public static string TempFolder()
        {
            return Path.Combine(Path.GetTempPath(), "HyperMediaTranscoder\\temp");
        }
    }
}