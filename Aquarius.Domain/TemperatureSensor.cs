namespace Aquarius.Domain
{
    public class TemperatureSensor
    {
        public Guid Id { get; set; }
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public List<Reading> Readings { get; set; } = new List<Reading>();

        public TemperatureSensor() { }
        public TemperatureSensor(Pond pond)
        {
            Id = Guid.NewGuid();
            PondId = pond.Id;
            Pond = pond;
        }
    }
}
