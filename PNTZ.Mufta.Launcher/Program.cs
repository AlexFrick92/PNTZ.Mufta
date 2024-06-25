using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using System.Windows;
using Promatis.DebuggingToolkit.IO;
using Promatis.DataPoint.Configuration;
using Promatis.DpProvider.OpcUa;
using Promatis.DataPoint;
using PNTZ.Mufta.Data;
using PNTZ.Mufta.RecipeHandling;

namespace PNTZ.Mufta.Launcher
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
      
            DpXmlConfiguration xmlConfiguration = new DpXmlConfiguration("DpConfig.xml");

            DpProviderConfigurator providerConfigurator = new DpProviderConfigurator();
            providerConfigurator.RegisterProvider(typeof(OpcUaProvider));
            providerConfigurator.ConfigureProviders(xmlConfiguration.ProviderConfiguration);
            providerConfigurator.StartProviders();

            DpProcessorConfigurator processorConfigurator = new DpProcessorConfigurator();
            RecipeLoader recipeLoader = new RecipeLoader();
            processorConfigurator.ConfigureProcessor(recipeLoader);
            processorConfigurator.ConfigureProcessor(xmlConfiguration.ProcessorConfiguration);

            DataPointConfigurator dataPointConfigurator = new DataPointConfigurator(providerConfigurator.ConfiguredProviders, processorConfigurator.ConfiguredProcessors);
            dataPointConfigurator.ConfigureDatapoints(xmlConfiguration.DataPointConfiguration);

            Cli cli = new Cli();

            cli.RegisterCommand("print", (args) => (cli as ICliProgram).WriteLine(args[0]));

            cli.RegisterCommand("load", (args) => recipeLoader.LoadRecipe(ConnectionRecipe.FromJson(args[0])));

            cli.RegisterCommand("init", (args) => recipeLoader.DpInitialized());


            cli.EnterInput("Hello!");

            MainViewModel mainViewModel = new MainViewModel(cli);
            MainView mainWin = new MainView(mainViewModel);

            Application app = new Application();
            app.Run(mainWin);

        }
    }
}
