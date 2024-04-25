using Promatis.Core;

namespace Promatis.MES.CLG.Console;


internal class Program
{
    static void Main(string[] args)
    {
        var config = new ModuleConfiguration();

        config.Run(AppRunningMode.Testing);
    }
}
