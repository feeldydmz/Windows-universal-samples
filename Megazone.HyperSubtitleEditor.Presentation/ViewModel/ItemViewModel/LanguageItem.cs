using Megazone.Cloud.Media.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class LanguageItem
    {
        public LanguageItem(Language language)
        {
            Code = language?.Code;
            Name = language?.Name;

            var codes = language?.Code?.Split('-') ?? new string[0];
            var names = language?.Name?.Split('-') ?? new string[0];

            if (codes.Length == 1)
            {
                LanguageCode = codes[0].Trim();
            }
            else if (codes.Length == 2)
            {
                LanguageCode = codes[0].Trim();
                CountryCode = codes[1].Trim();
            }
            else if (codes.Length == 3)
            {
                LanguageCode = $"{codes[0].Trim()}-{codes[1].Trim()}";
                CountryCode = codes[2].Trim();
            }

            if (names.Length == 1)
            {
                LanguageName = names[0].Trim();
            }
            else if (names.Length == 2)
            {
                LanguageName = names[0].Trim();
                CountryName = names[1].Trim();
            }
        }

        public string Code { get; }
        public string Name { get; }
        public string LanguageCode { get; }
        public string LanguageName { get; }
        public string CountryCode { get; }
        public string CountryName { get; }
    }
}