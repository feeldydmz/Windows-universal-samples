using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Language
{
    public class Language
    {
        [JsonProperty("Name")] public string Name { get; set; }

        [JsonProperty("NativeName")] public string NativeName { get; set; }

        [JsonProperty("Alpha2")] public string Alpha2 { get; set; }

        [JsonProperty("Alpha3")] public string Alpha3 { get; set; }
    }

    public class Country
    {
        [JsonProperty("Name")] public string Name { get; set; }

        [JsonProperty("NativeName")] public string NativeName { get; set; }

        [JsonProperty("Alpha2")] public string Alpha2 { get; set; }

        [JsonProperty("Alpha3")] public string Alpha3 { get; set; }
    }
}