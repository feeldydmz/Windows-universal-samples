namespace Megazone.Cloud.Media.Domain
{
    public class ProjectResponse
    {
        public ProjectResponse(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}
