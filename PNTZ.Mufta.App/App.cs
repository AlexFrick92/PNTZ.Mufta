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
using PNTZ.Mufta.App.ViewModel;
using PNTZ.Mufta.App.View;
using System.ComponentModel;
using PNTZ.Mufta.App.Domain;
using System.Collections.Generic;
using System.Windows.Navigation;



namespace PNTZ.Mufta.App
{
    internal class App : StagedApplication
    {
        DpFluentBuilder dataPointConfigurator = null;

        protected override async void BeforeInit()
        {
            AppInstance = this;

            CurrentDirectory = currentDirectory;

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


            ResultObserver = new JointResultObserver() { Name = "JointResultObserver"};
            CommonParamObserver commonParam = new CommonParamObserver() { Name = "CommonParamObserver"};

            MachineParameterObserver machineParameterObserver = new MachineParameterObserver() { Name = "MachineParamObserver" };

            dataPointConfigurator = new DpFluentBuilder()
                .SetLogger(logger)
                .AddConfiguration($"{currentDirectory}/DpConfig.xml")
                //.AddConfiguration($"{currentDirectory}/DpConfigStendPNTZ.xml")
                .SetProviders(new Type[] { typeof(OpcUaProvider) })
                .SetProcessors(new IDpProcessor[] { recipeLoader, heartbeat, heartbeatCheck, ResultObserver, commonParam, machineParameterObserver })
                //.SetProcessors(new IDpProcessor[] {machineParameterObserver, recipeLoader, heartbeat, heartbeatCheck })
                .Build();             


            cli.RegisterCommand("print", (args) => (cli as ICliProgram).WriteLine(args[0]));

            cli.RegisterCommand("init", (_) => recipeLoader.OnDpInitialized());
            cli.RegisterCommand("startpr", async (_) => await Task.Run(() => dataPointConfigurator.StartProviders()));
            cli.RegisterCommand("stoppr", (_) => dataPointConfigurator.StopProviders());

            cli.RegisterCommand("heartbeat", (_) => heartbeat.OnDpInitialized());




            MainViewModel mainViewModel = new MainViewModel(cli);
            mainWindow = new MainView(mainViewModel);
        }

        #region static stuff

        static public App AppInstance;

        public string CurrentDirectory { get; set; }
        public string RecipeFolder { get; set; } = "Рецепты";
        public string MachineParametersFolder { get; set; } = "Параметры машин";
        public string ResultFolder { get; set; } = "Результаты";

        public RecipeLoader CamRecipeLoader { get; set; }

        public Cli AppCli { get; set; }

        public ILogger AppLogger { get; set; }        

        public void SaveJointRecipe(JointRecipe joint)
        {
            if (joint == null)
                throw new ArgumentNullException();

            if (joint.Name.Trim() == "")
                throw new ArgumentException("Не задано имя рецепта");


            string recipeDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.RecipeFolder}";

            if (!Directory.Exists(recipeDirectory))
            {
                Directory.CreateDirectory(recipeDirectory);
            }

            string path = $"{recipeDirectory}/{joint.Name}.json";

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,  // Включаем красивое форматирование                    
                };

                JsonSerializer.Serialize<JointRecipe>(fs, joint, options);
                Console.WriteLine($"Рецепт: {joint.Name} сохранен в {path}");
            }
        }

        public List<JointRecipe> OpenJointRecipesFolder()
        {
            string mpDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.RecipeFolder}";

            var recipePaths = Directory.GetFiles(mpDirectory);

            List<JointRecipe> recipes = new List<JointRecipe> ();

            foreach(var path in recipePaths)
            {
                recipes.Add( OpenJointRecipe(path) );
            }

            return recipes;
        }

        public JointRecipe OpenJointRecipe(string path)
        {
            //string mpDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.RecipeFolder}";

            //string path = $"{mpDirectory}/{path}";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                return JsonSerializer.Deserialize<JointRecipe>(fs);
            }            
        }

        public void SaveMachineParameters(MachineParameters mp)
        {
            if (mp == null)
                throw new ArgumentNullException();            

            string mpDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.MachineParametersFolder}";

            if (!Directory.Exists(mpDirectory))
            {
                Directory.CreateDirectory(mpDirectory);
            }

            string path = $"{mpDirectory}/MpActual.json";

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<MachineParameters>(fs, mp);
                Console.WriteLine($"Параметры машин сохранены в {path}");
            }
        }

        public MachineParameters OpenMachineParameters()
        {

            string mpDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.MachineParametersFolder}";

            string path = $"{mpDirectory}/MpActual.json";

            if (!File.Exists(path))
            {
                if (!Directory.Exists(mpDirectory))
                {
                    Directory.CreateDirectory(mpDirectory);
                }

                MachineParameters mp = new MachineParameters();
                SaveMachineParameters(mp);
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                return JsonSerializer.Deserialize<MachineParameters>(fs);               
            }
        }


        public void SaveResult(JointResult result)
        {
            if (result == null)
                throw new ArgumentNullException();

            string recipeDirectory = $"{AppInstance.CurrentDirectory}/{AppInstance.ResultFolder}";

            if (!Directory.Exists(recipeDirectory))
            {
                Directory.CreateDirectory(recipeDirectory);
            }

            string path = $"{recipeDirectory}/Соединение_1.json";

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,  // Включаем красивое форматирование                    
                };
                JsonSerializer.Serialize<JointResult>(fs, result, options);
                Console.WriteLine($"Результат сохранен в {path}");
            }
        }

        public List<JointResult> OpenJointResult()
        {


            return default;
        }

        public ushort JointModeToMakeUpMode(JointMode jointMode)
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



        JointRecipe _loadedRecipe;
        public JointRecipe LoadedRecipe 
        { 
            get { return _loadedRecipe; }
            set
            {
                _loadedRecipe = value;
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(LoadedRecipe)));
            }
        }

        JointResult _lastJointResult;
        public JointResult LastJointResult
        {
            get { return _lastJointResult; }
            set
            {
                _lastJointResult = value;
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(LastJointResult)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;   
        
        public TqTnLen ActualTqTnLen { get; set; } = new TqTnLen();

        public JointResultObserver ResultObserver { get; set; }

        #endregion
    }
}
