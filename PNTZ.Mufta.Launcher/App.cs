using PNTZ.Mufta.Data;
using PNTZ.Mufta.Domain.RecipeHandling;
using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using PNTZ.Mufta.RecipeHandling;
using Promatis.DataPoint.Configuration;
using Promatis.DataPoint.Interface;
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
            DpBuilder dataPointConfigurator = null;
            Init += (_, _) =>
            {
                CliLogger logger = new CliLogger(_cli);

                RecipeCreator recipeCreator = new RecipeCreator();
                RecipeLoader recipeLoader = new RecipeLoader(logger);

                _cli.RegisterCommand("load", (args) =>
                {
                    recipeCreator.FromFile(args[0]);
                    recipeLoader.LoadRecipe(recipeCreator.Recipe);
                });
                _cli.RegisterCommand("load1", (args) => recipeLoader.Load());

                Heartbeat heartbeat = new Heartbeat(_cli) { Name = "Heartbeat1" };

                dataPointConfigurator = new DpBuilder(logger,
                    new string[] { _currentDirectory + "/DpConfig.xml" }, 
                    new Type[] {typeof(OpcUaProvider)},
                    null,
                    new IDpProcessor[] { recipeLoader, heartbeat }                    
                    );                             


                _cli.RegisterCommand("print", (args) => (_cli as ICliProgram).WriteLine(args[0]));
                
                _cli.RegisterCommand("init", (_) => recipeLoader.DpInitialized());
                _cli.RegisterCommand("startpr", async (_) => await Task.Run(() => dataPointConfigurator.StartProviders()));
                _cli.RegisterCommand("stoppr", (_) => dataPointConfigurator.StopProviders());

                _cli.RegisterCommand("heartbeat", (_) => heartbeat.DpInitialized());
                

                MainViewModel mainViewModel = new MainViewModel(_cli);
                _mainWindow = new MainView(mainViewModel);
            };

            BeforeExit += async (sender, args) => await Task.Run(() => dataPointConfigurator?.StopProviders());
        }
    }
}
