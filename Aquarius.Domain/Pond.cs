namespace Aquarius.Domain
{
    public class Pond
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; } // Maximum number of tilapia
        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }
        public List<Sensor> Sensors { get; set; } = new List<Sensor>();

        public Pond(string name, int capacity, Farm farm)
        {
            Id = Guid.NewGuid();
            Name = name;
            Capacity = capacity;
            FarmId = farm.Id;
            Farm = farm;
        }
    }
}