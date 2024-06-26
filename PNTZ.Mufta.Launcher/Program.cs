using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using System.Windows;
using Promatis.DebuggingToolkit.IO;
using Promatis.DataPoint.Configuration;
using Promatis.DpProvider.OpcUa;
using Promatis.DataPoint;
using PNTZ.Mufta.Data;
using PNTZ.Mufta.RecipeHandling;
using Promatis.DebuggingToolkit.Logging;
using TorqueControl.RecipeHandling;
using Promatis.DpProcessor.PlcSystem.Connection;

namespace PNTZ.Mufta.Launcher
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            GreetingView greetingView = new GreetingView();
            greetingView.Show();


            Task.Delay(1000).Wait();

            Cli cli = new Cli();
            CliLogger logger = new CliLogger(cli);


            DpXmlConfiguration xmlConfiguration = new DpXmlConfiguration("DpConfig.xml");

            DpProviderConfigurator providerConfigurator = new DpProviderConfigurator();
            providerConfigurator.RegisterProvider(typeof(OpcUaProvider));
            providerConfigurator.ConfigureProviders(xmlConfiguration.ProviderConfiguration);
            providerConfigurator.StartProviders();

            DpProcessorConfigurator processorConfigurator = new DpProcessorConfigurator();
            
            RecipeLoader recipeLoader = new RecipeLoader(logger);
            Heartbeat heartbeat = new Heartbeat(cli) { Name = "Heartbeat1"};
            processorConfigurator.ConfigureProcessor(recipeLoader);
            processorConfigurator.ConfigureProcessor(heartbeat);
            processorConfigurator.ConfigureProcessor(xmlConfiguration.ProcessorConfiguration);

            DataPointConfigurator dataPointConfigurator = new DataPointConfigurator(providerConfigurator.ConfiguredProviders, processorConfigurator.ConfiguredProcessors);
            dataPointConfigurator.ConfigureDatapoints(xmlConfiguration.DataPointConfiguration);

            cli.RegisterCommand("print", (args) => (cli as ICliProgram).WriteLine(args[0]));

            cli.RegisterCommand("load", (args) => ((ILoadRecipe)recipeLoader).LoadRecipe(ConnectionRecipe.FromJson(args[0])));
            cli.RegisterCommand("init", (args) => recipeLoader.DpInitialized());

            cli.RegisterCommand("heartbeat", (args) => heartbeat.DpInitialized());
            cli.RegisterCommand("setstring", (args) => recipeLoader.Pipe_type.Value = args[0]);

            MainViewModel mainViewModel = new MainViewModel(cli);
            MainView mainWin = new MainView(mainViewModel);

            Application app = new Application();
            
            cli.RegisterCommand("exit", async (args) =>
            {
                cli.WriteLine("Exiting...");
                await Task.Delay(5000);                    
                app.Shutdown();                
                
            });

            mainWin.Show();
            greetingView.Hide();
            mainWin.Closed += (_, _) => greetingView.Close();
            
            app.Run();            
        }
    }
}
