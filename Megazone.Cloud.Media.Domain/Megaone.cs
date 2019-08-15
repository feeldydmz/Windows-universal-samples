namespace Megazone.Cloud.Media.Domain
{
    public class Megaone
    {
        public Megaone(string id, string name, string username)
        {
            Id = id;
            Name = name;
            Username = username;
        }

        public string Id { get; }
        public string Name { get; }
        public string Username { get; }
    }
}