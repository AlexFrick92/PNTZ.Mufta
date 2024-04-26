using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.Core.Modularity;

namespace Promatis.MES.CLG.Console;


internal class Program
{
    static void Main(string[] args)
    {
        var logger = new ConsoleLogger();

        var config = new ModuleConfiguration();


        var environment = new ModuleSettings("TestCLG", logger);
        config.Initialize(environment, logger);
        config.Run(AppRunningMode.Testing);
    }
}
