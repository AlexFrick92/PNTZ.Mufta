using PNTZ.Mufta.Data;
using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using PNTZ.Mufta.RecipeHandling;
using Promatis.DataPoint;
using Promatis.DataPoint.Configuration;
using Promatis.DebuggingToolkit.IO;
using Promatis.DebuggingToolkit.Logging;
using Promatis.Desktop.Application;
using Promatis.DpProcessor.PlcSystem.Connection;
using Promatis.DpProvider.OpcUa;

namespace PNTZ.Mufta.Launcher
{
    internal class App : StagedApplication
    {
        public App()
        {
            Init += (_, _) =>
            {
                CliLogger logger = new CliLogger(_cli);

                DpXmlConfiguration xmlConfiguration = new DpXmlConfiguration(_currentDirectory +  "/DpConfig.xml");

                DpProviderConfigurator providerConfigurator = new DpProviderConfigurator(logger);
                providerConfigurator.RegisterProvider(typeof(OpcUaProvider));
                providerConfigurator.ConfigureProviders(xmlConfiguration.ProviderConfiguration);

                DpProcessorConfigurator processorConfigurator = new DpProcessorConfigurator();

                RecipeLoader recipeLoader = new RecipeLoader(logger);
                _cli.RegisterCommand("load", (args) => recipeLoader.LoadRecipe(args[0]));
                processorConfigurator.ConfigureProcessor(recipeLoader);
                
                
                Heartbeat heartbeat = new Heartbeat(_cli) { Name = "Heartbeat1" };
                processorConfigurator.ConfigureProcessor(heartbeat);

                processorConfigurator.ConfigureProcessor(xmlConfiguration.ProcessorConfiguration);

                DataPointConfigurator dataPointConfigurator = new DataPointConfigurator(providerConfigurator.ConfiguredProviders, processorConfigurator.ConfiguredProcessors);
                dataPointConfigurator.ConfigureDatapoints(xmlConfiguration.DataPointConfiguration);


                _cli.RegisterCommand("print", (args) => (_cli as ICliProgram).WriteLine(args[0]));

                
                _cli.RegisterCommand("init", (_) => recipeLoader.DpInitialized());
                _cli.RegisterCommand("startpr", async (_) => await Task.Run(() => providerConfigurator.StartProviders()));
                _cli.RegisterCommand("stoppr", (_) => providerConfigurator.StopProviders());

                _cli.RegisterCommand("heartbeat", (_) => heartbeat.DpInitialized());
                _cli.RegisterCommand("setstring", (args) => recipeLoader.Pipe_type.Value = args[0]);

                MainViewModel mainViewModel = new MainViewModel(_cli);
                _mainWindow = new MainView(mainViewModel);
            };
        }
    }
}
