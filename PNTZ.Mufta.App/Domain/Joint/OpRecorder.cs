using Promatis.DataPoint.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.App.Domain.Joint
{
    public class OpRecorder : IDpProcessor
    {
        public string Name { get; set; } = "OpRecorder1";    
        public void OnDpInitialized()
        {
            
        }
    }
}
