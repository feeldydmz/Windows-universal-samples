namespace Megazone.Cloud.Media.Domain
{
    public class StageResponse
    {
        public StageResponse(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}
