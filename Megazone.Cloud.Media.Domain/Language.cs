using System;

namespace Megazone.Cloud.Media.Domain
{
    [Serializable]
    public class Language
    {
        public Language(string code, string name)
        {
            var codes = code?.Split('-') ?? new string[0];
            var names = name?.Split('-') ?? new string[0];

            LanguageCode = codes.Length == 2 ? codes[0].Trim() : string.Empty;
            LanguageName = names.Length == 2 ? names[0].Trim() : string.Empty;
            CountryCode = codes.Length == 2 ? codes[1].Trim() : string.Empty;
            CountryName = names.Length == 2 ? names[1].Trim() : string.Empty;
        }

        public string LanguageCode { get; }
        public string LanguageName { get; }
        public string CountryCode { get; }
        public string CountryName { get; }
    }
}