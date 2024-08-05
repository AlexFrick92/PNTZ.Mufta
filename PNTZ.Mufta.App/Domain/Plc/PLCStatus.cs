using Toolkit.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Domain.PLC
{
    public class PLCStatus
    {
        ICliProgram _cli;
        public PLCStatus(ICliProgram cli)
        {
            _cli = cli;
            _cli.RegisterCommand("testplc", (_) => TestPlc());
        }

        void TestPlc()
        {
            _cli.WriteLine(ToString());
        }
        public string ToString()
        {
            return "Всё супер!";
        }
    }
}
