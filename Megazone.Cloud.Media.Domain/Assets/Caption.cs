namespace Megazone.Cloud.Media.Domain.Assets
{
    public class Caption : IAssetElement
    {
        public Caption(string id, bool isDraft, bool isPreferred, string language, string country, string kind, string label, string url)
        {
            Id = id;
            IsDraft = isDraft;
            IsPreferred = isPreferred;
            Language = language;
            Country = country;
            Kind = kind;
            Label = label;
            Url = url;
        }

        public string Id { get; }
        public bool IsDraft { get; }
        public bool IsPreferred { get; }
        public string Language { get; }
        public string Country { get; }
        public string Kind { get; }
        public string Label { get; }
        public string Url { get; }
    }
}
