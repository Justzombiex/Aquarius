namespace Aquarius.Domain
{
    public class LevelSensor
    {
        public Guid Id { get; set; }
        public bool FullPond { get; set; }
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public LevelSensor() { }
        public LevelSensor(bool fullPond, Pond pond)
        {
            Id = Guid.NewGuid();
            FullPond = fullPond;
            PondId = pond.Id;
            Pond = pond;
        }
    }
}
