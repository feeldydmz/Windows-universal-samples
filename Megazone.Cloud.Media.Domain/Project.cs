using System;

namespace Megazone.Cloud.Media.Domain
{
    public class Project
    {
        public Project(string id,
            string name,
            string description,
            bool usePlayout,
            bool isActive,
            DateTime createdAt,
            string createdById,
            string createdByName,
            string createdByUsername,
            DateTime updatedAt,
            string updatedById,
            string updatedByName,
            string updatedByUsername)
        {
            Id = id;
            Name = name;
            Description = description;
            UsePlayout = usePlayout;
            IsActive = isActive;
            CreatedAt = createdAt;
            CreatedById = createdById;
            CreatedByName = createdByName;
            CreatedByUsername = createdByUsername;
            UpdatedAt = updatedAt;
            UpdatedById = updatedById;
            UpdatedByName = updatedByName;
            UpdatedByUsername = updatedByUsername;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool UsePlayout { get; }
        public bool IsActive { get; }

        public DateTime CreatedAt { get; }
        public string CreatedById { get; }
        public string CreatedByName { get; }
        public string CreatedByUsername { get; }
        public DateTime UpdatedAt { get; }
        public string UpdatedById { get; }
        public string UpdatedByName { get; }
        public string UpdatedByUsername { get; }
    }
}