namespace Aquarius.Domain
{
    public class Pond
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; } 
        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }

        public TemperatureSensor TemperatureSensor { get; set; }

        public LevelSensor LevelSensor { get; set; }

        public Pond() { }

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
