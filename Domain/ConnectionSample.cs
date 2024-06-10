using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ConnectionSample
    {
        public Guid ConnectionID { get; private set; }
        public float Torque { get; private set; }
        public float Turns { get; private set; }
        public float Length { get; private set; }
        public DateTime TimeStamp { get; private set; }
    }
}
