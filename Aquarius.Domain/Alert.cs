using Aquarius.Domain;

namespace Aquarius.Domain
{
    public class Alert
    {
        public Guid Id { get; set; }
        public string Message { get; set; } // Alert description
        public DateTime TimeStamp { get; set; }
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public VariableType VariableType { get; set; } // "Temperature" or "Level"

        public Alert(string message, VariableType type, DateTime timestamp, Pond pond)
        {
            Id = Guid.NewGuid();
            Message = message;
            TimeStamp = timestamp;
            PondId = pond.Id;
            Pond = pond;
            VariableType = type;
        }
    }
}