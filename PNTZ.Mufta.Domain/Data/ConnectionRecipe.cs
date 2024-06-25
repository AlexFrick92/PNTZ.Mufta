using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PNTZ.Mufta.Data
{
    public class ConnectionRecipe : TorqueControl.Data.ConnectionRecipe<ConnectionRecipe>
    {
        public float HEAD_OPEN_PULSES { get; set; }

        public float TURNS_BREAK { get; set; }
    }

}
