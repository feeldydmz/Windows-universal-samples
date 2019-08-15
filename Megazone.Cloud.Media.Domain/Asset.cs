namespace Megazone.Cloud.Media.Domain
{
    public class Asset
    {
        public Asset(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}