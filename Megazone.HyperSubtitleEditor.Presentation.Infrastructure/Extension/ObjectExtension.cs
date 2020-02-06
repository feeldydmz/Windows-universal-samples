using System;
using System.IO;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension
{
    public static class ObjectExtension
    {
        private const string CompanyName = "Megazone";
        private const string ApplicationName = "CloudPlex Media Caption Editor";

        public static string AppDataPath(this object obj)
        {
            return AppDataPath();
        }

        public static string AppDataPath()
        {
            return
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{CompanyName}\{ApplicationName}\";
        }

        public static string LogDirPath(this object obj)
        {
            return LogDirPath();
        }

        public static string LogDirPath()
        {
            return
                $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{CompanyName}\{ApplicationName}\logs";
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

        public static string TempFolder(this object obj)
        {
            return TempFolder();
        }

        public static string TempFolder()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"{ApplicationName}\\temp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            return tempPath;
        }
    }
}