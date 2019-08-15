namespace Megazone.Cloud.Media.Domain
{
    public class Project
    {
        public Project(string id, string name, string description, bool isActive)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsActive { get; }
    }
}