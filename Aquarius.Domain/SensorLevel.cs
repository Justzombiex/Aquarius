using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarius.Domain
{
    public class SensorLevel
    {
        public Guid Id { get; set; }
        public bool FullPond { get; set; }
        public Guid PondId { get; set; }
        public Pond Pond { get; set; }
        public List<Reading> Readings { get; set; } = new List<Reading>();

        public SensorLevel() { }
        public SensorLevel(bool fullPond ,Pond pond)
        {
            Id = Guid.NewGuid();
            FullPond = fullPond;
            PondId = pond.Id;
            Pond = pond;
        }
    }
}
