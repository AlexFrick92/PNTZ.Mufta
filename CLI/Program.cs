using Cam;
using Promatis.DataPoint;
using Promatis.DataPoint.Configuration;
using Promatis.DpProvider.OpcUa;
using TorqueControl.RecipeHandling;

namespace CLI
{
    internal class Program
    {
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

            RecipeManager recipeManager = new RecipeManager();


            while (true)
            {

                var input = System.Console.ReadLine();
                if (input == "load")
                {
                    recipeLoader.LoadRecipe(recipeManager.CreateRecipe());
                }
                else if (input == "subs")
                {
                    recipeLoader.DpInitialized();
                }
            }
        }
    }
}
