using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Language
{
    public class Language
    {
        public Language()
        {
            CountryInfo = new Country();
        }

        public string Name { get; set; }

        public string NativeName { get; set; }

        public string Alpha2 { get; set; }

        public Country CountryInfo { get; set; }
    }

    public class Country
    {
         public string Name { get; set; }

        public string Alpha2 { get; set; }
    }
}