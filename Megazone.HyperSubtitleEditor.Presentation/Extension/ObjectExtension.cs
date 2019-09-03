using System.IO;

namespace Megazone.HyperSubtitleEditor.Presentation.Extension
{
    internal static class ObjectExtension
    {
        public static string GetTempFolderPath(this object self)
        {
            var folderPath = VideoStudio.Presentation.Common.Infrastructure.Extension.ObjectExtension.TempFolder();
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                return folderPath;
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}