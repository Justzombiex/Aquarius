namespace Aquarius.Domain {
    public class Reading
    {
        public Guid Id { get; set; }
        public double Value { get; set; } // Reading value (temperature or level)
        public DateTime Timestamp { get; set; }
        public Guid SensorId { get; set; }
        public Sensor Sensor { get; set; }

        public Reading() { }
        public Reading(double value, DateTime timestamp, Sensor sensor)
        {
            Id = Guid.NewGuid();
            Value = value;
            Timestamp = timestamp;
            SensorId = sensor.Id;
            Sensor = sensor;
        }
    }
}