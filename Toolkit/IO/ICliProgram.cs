using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.IO
{
    public delegate void CommandExecute(params string[] args);
    public interface ICliProgram
    {
        void RegisterCommand(string commandName, CommandExecute command = null);
        void WriteLine(string line);
    }
}
