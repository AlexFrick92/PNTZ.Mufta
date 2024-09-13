using System;
using System.Threading.Tasks;

using Toolkit.Logging;
using Toolkit.IO;

using Desktop.Application;

using DpConnect.Interface;
using DpConnect.Configuration;
using DpConnect.Provider.OpcUa;

using PNTZ.Mufta.App.Domain.Plc;
using PNTZ.Mufta.App.Domain.Joint;
using PNTZ.Mufta.App.ViewModel.Chart;
using PNTZ.Mufta.App.ViewModel;
using PNTZ.Mufta.App.View;

using static PNTZ.Mufta.App.Global.Vars;


namespace PNTZ.Mufta.App
{
    internal class App : StagedApplication
    {
        DpFluentBuilder dataPointConfigurator = null;

        protected override async void BeforeInit()
        {
            CurrentDirectory = currentDirectory;

            RecipeFolder = "Рецепты";

            AppCli = cli;

            await Task.Run(() => dataPointConfigurator?.StopProviders());
        }

        protected override void Init()
        {

            CliLogger logger = new CliLogger(cli);

            AppLogger = logger;

            PLCStatus status = new PLCStatus(cli);

            RecipeLoader recipeLoader = new RecipeLoader(logger);

            CamRecipeLoader = recipeLoader;        

            HeartbeatMake heartbeat = new HeartbeatMake(cli) { Name = "Heartbeat1" };
            HeartbeatCheck heartbeatCheck = new HeartbeatCheck(cli) { Name = "HeartbeatCheck1" };


            OpRecorder opRecorder = new OpRecorder("OpRecorder", cli);
            ChartViewModel chartViewModel = new ChartViewModel(opRecorder);


            dataPointConfigurator = new DpFluentBuilder()
                .SetLogger(logger)
                .AddConfiguration($"{currentDirectory}/DpConfig.xml")
                .SetProviders(new Type[] { typeof(OpcUaProvider) })
                .SetProcessors(new IDpProcessor[] { recipeLoader, heartbeat, heartbeatCheck, opRecorder, chartViewModel })
                .Build();             


            cli.RegisterCommand("print", (args) => (cli as ICliProgram).WriteLine(args[0]));

            cli.RegisterCommand("init", (_) => recipeLoader.OnDpInitialized());
            cli.RegisterCommand("startpr", async (_) => await Task.Run(() => dataPointConfigurator.StartProviders()));
            cli.RegisterCommand("stoppr", (_) => dataPointConfigurator.StopProviders());

            cli.RegisterCommand("heartbeat", (_) => heartbeat.OnDpInitialized());




            MainViewModel mainViewModel = new MainViewModel(cli, chartViewModel);
            mainWindow = new MainView(mainViewModel);
        }

       
        
    }
}
