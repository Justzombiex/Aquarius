namespace Aquarius.Domain
{
    public class Farm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<Pond> Ponds { get; set; } = new List<Pond>();

        public Farm(string name, string location)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
        }
    }
}