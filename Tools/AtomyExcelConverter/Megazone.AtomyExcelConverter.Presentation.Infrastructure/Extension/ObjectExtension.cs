using System;

namespace Megazone.AtomyExcelConverter.Presentation.Infrastructure.Extension
{
    public static class ObjectExtension
    {
        public static string AtomyExcelConverterAppDataPath(this object obj)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                   "\\Megazone\\AtomyExcelConverter\\";
        }
    }
}
