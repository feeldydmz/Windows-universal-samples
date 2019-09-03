using System;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension
{
    public static class ObjectExtension
    {
        public static string AppDataPath(this object obj)
        {
            return AppDataPath();
        }

        public static string AppDataPath()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Megazone\\Hyper Subtitle Editor\";
        }
    }
}