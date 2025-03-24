using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarius.Domain
{
    public class SensorTemperature
    {
        public Guid Id { get; set; }
       
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public List<Reading> Readings { get; set; } = new List<Reading>();

        public SensorTemperature() { }
        public SensorTemperature(Pond pond)
        {
            Id = Guid.NewGuid();
            
            PondId = pond.Id;
            Pond = pond;
        }
    }
}
