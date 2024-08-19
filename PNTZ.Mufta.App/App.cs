using PNTZ.Mufta.Data;
using PNTZ.Mufta.Domain.PLC;
using PNTZ.Mufta.Domain.RecipeHandling;
using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using PNTZ.Mufta.Launcher.ViewModel.Chart;
using PNTZ.Mufta.RecipeHandling;
using Promatis.DataPoint.Configuration;
using Promatis.DataPoint.Interface;
using Toolkit.IO;
using Toolkit.Logging;
using Promatis.Desktop.Application;
using Promatis.DpProcessor.PlcSystem.Connection;
using Promatis.DpProvider.OpcUa;
using PNTZ.Mufta.App.Domain.Joint;
using PNTZ.Mufta.App;

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
                
                PLCStatus status = new PLCStatus(_cli);

                RecipeLoader recipeLoader = new RecipeLoader(logger);
                _cli.RegisterCommand("load", (args) =>
                {
                    RecipeCreator recipeCreator = new RecipeCreator(args[0]);                    
                    recipeLoader.LoadRecipe(recipeCreator.Recipe);
                });
                _cli.RegisterCommand("load1", (args) => recipeLoader.Load());

                HeartbeatGenerate heartbeat = new HeartbeatGenerate(_cli) { Name = "Heartbeat1" };
                HeartbeatCheck heartbeatCheck = new HeartbeatCheck(_cli) { Name = "HeartbeatCheck1" };


                OpRecorder opRecorder = new OpRecorder("OpRecorder", _cli);
                ChartViewModel chartViewModel = new ChartViewModel(opRecorder);

                dataPointConfigurator = new DpBuilder(logger,
                    new string[] { _currentDirectory + "/DpConfig.xml" }, 
                    new Type[] {typeof(OpcUaProvider)},
                    null,
                    new IDpProcessor[] { recipeLoader, heartbeat, heartbeatCheck, opRecorder, chartViewModel }                    
                    );                             

                _cli.RegisterCommand("print", (args) => (_cli as ICliProgram).WriteLine(args[0]));
                
                _cli.RegisterCommand("init", (_) => recipeLoader.OnDpInitialized());
                _cli.RegisterCommand("startpr", async (_) => await Task.Run(() => dataPointConfigurator.StartProviders()));
                _cli.RegisterCommand("stoppr", (_) => dataPointConfigurator.StopProviders());

                _cli.RegisterCommand("heartbeat", (_) => heartbeat.OnDpInitialized());
                



                MainViewModel mainViewModel = new MainViewModel(_cli, chartViewModel);
                _mainWindow = new MainView(mainViewModel);
            };

            BeforeExit += async (sender, args) => await Task.Run(() => dataPointConfigurator?.StopProviders());
        }
    }
}
