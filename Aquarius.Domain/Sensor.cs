namespace Aquarius.Domain
{
    public class Sensor
    {
        public Guid Id { get; set; }
        public VariableType VariableType { get; set; } // "Temperature" or "Level"
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public List<Reading> Readings { get; set; } = new List<Reading>();

        public Sensor() { }
        public Sensor(VariableType type, Pond pond)
        {
            Id = Guid.NewGuid();
            VariableType = type;
            PondId = pond.Id;
            Pond = pond;
        }
    }
}