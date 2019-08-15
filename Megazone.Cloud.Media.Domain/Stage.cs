using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class Stage
    {
        public Stage(string id, string name, string endpoint, bool isActive, IEnumerable<Project> projects)
        {
            Id = id;
            Name = name;
            Endpoint = endpoint;
            IsActive = isActive;
            Projects = projects;
        }

        public string Id { get; }
        public string Name { get; }
        public string Endpoint { get; }
        public bool IsActive { get; }
        public IEnumerable<Project> Projects { get; }
    }
}