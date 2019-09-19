using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Language
{
    public static class LanguageParser
    {
        private static string ReadFromFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) return File.ReadAllText(filePath);
            return null;
        }

        private static string GetDefaultLanguages()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName()
                                   .Name +
                               ".Language.LanguageInfo.json";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static IEnumerable<Language> TryConvert(string jsonText)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonText))
                    return JsonConvert.DeserializeObject<IEnumerable<Language>>(jsonText);
            }
            catch
            {
                // ignored
            }
            return null;
        }

        public static IEnumerable<Language> GetLanguages(string filePath = null)
        {
            var convertedLanguages = TryConvert(ReadFromFile(filePath));
            return convertedLanguages ?? TryConvert(GetDefaultLanguages());
        }
    }
}