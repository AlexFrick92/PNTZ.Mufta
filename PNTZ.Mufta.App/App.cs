using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

using Promatis.Core.Logging;

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
using System.ComponentModel;



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

        #region static stuff

        static public string CurrentDirectory { get; set; }
        static public string RecipeFolder { get; set; }

        static public RecipeLoader CamRecipeLoader { get; set; }

        static public Cli AppCli { get; set; }

        static public ILogger AppLogger { get; set; }
        
        static public JointRecipe LoadedRecipe { get; private set ; }

        static public void SaveJointRecipe(JointRecipe joint)
        {
            if (joint == null)
                throw new ArgumentNullException();

            if (joint.Name.Trim() == "")
                throw new ArgumentException("Не задано имя рецепта");


            string recipeDirectory = $"{App.CurrentDirectory}/{App.RecipeFolder}";

            if (!Directory.Exists(recipeDirectory))
            {
                Directory.CreateDirectory(recipeDirectory);
            }

            string path = $"{recipeDirectory}/{joint.Name}.json";

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<JointRecipe>(fs, joint);
                Console.WriteLine($"Рецепт: {joint.Name} сохранен в {path}");
            }
        }
        static public ushort JointModeToMakeUpMode(JointMode jointMode)
        {
            switch (jointMode)
            {
                case JointMode.Torque: return 0;

                case JointMode.TorqueShoulder: return 0;

                case JointMode.Length: return 1;

                case JointMode.TorqueLength: return 1;

                case JointMode.Jval: return 2;

                case JointMode.TorqueJVal: return 2;
            }

            return 0;
        }

        static public void UpdateLoadedRecipe(JointRecipe jointRecipe)
        {
            App.LoadedRecipe = jointRecipe;
            PropertyChanged(null, new PropertyChangedEventArgs(nameof(LoadedRecipe)));
        }        

        public static event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
