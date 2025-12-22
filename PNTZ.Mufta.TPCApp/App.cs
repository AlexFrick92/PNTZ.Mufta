using Desktop.Application;


using DpConnect;
using DpConnect.Building;
using DpConnect.Configuration.Xml;
using DpConnect.Connection;
using DpConnect.OpcUa;

using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View;
using PNTZ.Mufta.TPCApp.ViewModel;

using Promatis.Core;
using Promatis.Core.Logging;
using Promatis.Core.Results;
using Promatis.IoC.DryIoc;
using Promatis.Logging.NLog;

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using Toolkit.IO;
using Toolkit.Logging;



namespace PNTZ.Mufta.TPCApp
{
    /// <summary>
    /// Класс унаследованный от StagedAplication
    /// StagedApplication - база для wpf приложения, в которой организована
    /// поэтапный запуск приложения. 
    /// Сначала показывается окно загрузки
    /// В это время можно запустить процесс инициализации
    /// Затем открывается основное окно    
    /// </summary>
    internal class App : StagedApplication
    {        

        static public App AppInstance;
        public string CurrentDirectory { get; private set; }
        public IDpBuilder DpBuilder { get; private set; }
        public IDpConnectionManager DpConnectionManager { get => DpBuilder.ConnectionManager; }
        public IDpWorkerManager DpWorkerManager { get => DpBuilder.WorkerManager; }

        public ILogger Logger { get; private set; }

        public IIoCContainer container { get; private set; }

        protected override void BeforeInit()
        {
            AppInstance = this;
            CurrentDirectory = currentDirectory;

            // Загружаем настройки приложения
            LoadAppSettings();

            // Загружаем цветовую схему приложения
            LoadAppColors();

            // Загружаем настройки шрифтов приложения
            LoadAppFonts();

            // Загружаем тексты лейблов приложения
            LoadAppLabels();

            // Загружаем настройки разметки приложения
            LoadAppLayouts();

            Logger = NLogManager.GetLogger("_logger");
            CliLogger cliLogger = new CliLogger(cli, Logger);

            //В контейнере регистрируем все необходимые классы для работы с ПЛК
            container = new DryIocContainer();

            container.RegisterInstance(container);

            container.RegisterInstance<ICliProgram>(cli);
            container.RegisterInstance<ICliUser>(cli);
            
            container.RegisterInstance<ILogger>(cliLogger);           

            Logger.Info("********** ЗАПУСК *************");            
            
            container.RegisterSingleton(typeof(LocalRepository), typeof(LocalRepository));
            
            container.Register<IDpConfigurableConnection<OpcUaConnectionConfiguration>, OpcUaConnection>();
            container.Register<MakeHeartBeat, MakeHeartBeat>();
            container.Register<RecipeDpWorker, RecipeDpWorker>();
            container.Register<MachineParamFromPlc, MachineParamFromPlc>();
            container.Register<HeartbeatCheck, HeartbeatCheck>();
            container.Register<JointProcessDpWorker, JointProcessDpWorker>();
            container.Register<SensorStatusDpWorker, SensorStatusDpWorker>();
            
            container.RegisterSingleton(typeof(IDpConnectionManager), typeof(ContainerizedConnectionManager));
            container.RegisterSingleton(typeof(IDpWorkerManager), typeof(ContainerizedWorkerManager));
            container.RegisterSingleton<IDpBinder, DpBinder>();
            container.Register<IDpBuilder, DpXmlBuilder>();

            container.Register<MainViewModel, MainViewModel>();


            Logger = container.Resolve<ILogger>();
                        
            DpBuilder = container.Resolve<IDpBuilder>();            
        }

        protected override void Init()
        {
            DpBuilder.Build();
            
            mainWindow = new MainView();
            mainWindow.DataContext = container.Resolve<MainViewModel>();
        }

        protected override void AfterInit()
        {

        }

        /// <summary>
        /// Загрузка цветовой схемы приложения
        /// 1. Загружаем встроенный Styles/AppColors.xaml (дефолтные цвета)
        /// 2. Пытаемся загрузить внешний Config/AppColors.xaml (опциональный override)
        /// </summary>
        private void LoadAppColors()
        {
            // 1. Загружаем дефолтные цвета из встроенного ресурса
            try
            {
                var defaultColorsUri = new Uri("pack://application:,,,/Styles/AppColors.xaml", UriKind.Absolute);
                var defaultColors = new ResourceDictionary { Source = defaultColorsUri };
                this.Resources.MergedDictionaries.Add(defaultColors);
            }
            catch (Exception ex)
            {
                // Критичная ошибка - дефолтные цвета должны быть всегда
                throw new InvalidOperationException("Не удалось загрузить Styles/AppColors.xaml", ex);
            }

            // 2. Пытаемся загрузить кастомные цвета из внешнего файла
            string customColorsPath = Path.Combine(CurrentDirectory, "Config", "AppColors.xaml");

            if (File.Exists(customColorsPath))
            {
                try
                {
                    using (var stream = File.OpenRead(customColorsPath))
                    {
                        var customColors = (ResourceDictionary)XamlReader.Load(stream);
                        this.Resources.MergedDictionaries.Add(customColors);
                    }
                }
                catch (Exception ex)
                {
                    // Не критично - используем дефолтные цвета
                    // Логирование будет позже, когда Logger инициализируется
                    Console.WriteLine($"Предупреждение: не удалось загрузить Config/AppColors.xaml - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Загрузка настроек шрифтов приложения
        /// 1. Загружаем встроенный Styles/AppFonts.xaml (дефолтные настройки шрифтов)
        /// 2. Пытаемся загрузить внешний Config/AppFonts.xaml (опциональный override)
        /// </summary>
        private void LoadAppFonts()
        {
            // 1. Загружаем дефолтные настройки шрифтов из встроенного ресурса
            try
            {
                var defaultFontsUri = new Uri("pack://application:,,,/Styles/AppFonts.xaml", UriKind.Absolute);
                var defaultFonts = new ResourceDictionary { Source = defaultFontsUri };
                this.Resources.MergedDictionaries.Add(defaultFonts);
            }
            catch (Exception ex)
            {
                // Критичная ошибка - дефолтные шрифты должны быть всегда
                throw new InvalidOperationException("Не удалось загрузить Styles/AppFonts.xaml", ex);
            }

            // 2. Пытаемся загрузить кастомные настройки шрифтов из внешнего файла
            string customFontsPath = Path.Combine(CurrentDirectory, "Config", "AppFonts.xaml");

            if (File.Exists(customFontsPath))
            {
                try
                {
                    using (var stream = File.OpenRead(customFontsPath))
                    {
                        var customFonts = (ResourceDictionary)XamlReader.Load(stream);
                        this.Resources.MergedDictionaries.Add(customFonts);
                    }
                }
                catch (Exception ex)
                {
                    // Не критично - используем дефолтные настройки шрифтов
                    // Логирование будет позже, когда Logger инициализируется
                    Console.WriteLine($"Предупреждение: не удалось загрузить Config/AppFonts.xaml - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Загрузка настроек приложения
        /// 1. Загружаем встроенный Styles/AppSettings.xaml (дефолтные настройки)
        /// 2. Пытаемся загрузить внешний Config/AppSettings.xaml (опциональный override)
        /// </summary>
        private void LoadAppSettings()
        {
            // 1. Загружаем дефолтные настройки из встроенного ресурса
            try
            {
                var defaultSettingsUri = new Uri("pack://application:,,,/Styles/AppSettings.xaml", UriKind.Absolute);
                var defaultSettings = new ResourceDictionary { Source = defaultSettingsUri };
                this.Resources.MergedDictionaries.Add(defaultSettings);
            }
            catch (Exception ex)
            {
                // Критичная ошибка - дефолтные настройки должны быть всегда
                throw new InvalidOperationException("Не удалось загрузить Styles/AppSettings.xaml", ex);
            }

            // 2. Пытаемся загрузить кастомные настройки из внешнего файла
            string customSettingsPath = Path.Combine(CurrentDirectory, "Config", "AppSettings.xaml");

            if (File.Exists(customSettingsPath))
            {
                try
                {
                    using (var stream = File.OpenRead(customSettingsPath))
                    {
                        var customSettings = (ResourceDictionary)XamlReader.Load(stream);
                        this.Resources.MergedDictionaries.Add(customSettings);
                    }
                }
                catch (Exception ex)
                {
                    // Не критично - используем дефолтные настройки
                    // Логирование будет позже, когда Logger инициализируется
                    Console.WriteLine($"Предупреждение: не удалось загрузить Config/AppSettings.xaml - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Загрузка текстов лейблов приложения
        /// 1. Загружаем встроенный Styles/AppLabels.xaml (дефолтные тексты)
        /// 2. Пытаемся загрузить внешний Config/AppLabels.xaml (опциональный override)
        /// </summary>
        private void LoadAppLabels()
        {
            // 1. Загружаем дефолтные тексты из встроенного ресурса
            try
            {
                var defaultLabelsUri = new Uri("pack://application:,,,/Styles/AppLabels.xaml", UriKind.Absolute);
                var defaultLabels = new ResourceDictionary { Source = defaultLabelsUri };
                this.Resources.MergedDictionaries.Add(defaultLabels);
            }
            catch (Exception ex)
            {
                // Критичная ошибка - дефолтные тексты должны быть всегда
                throw new InvalidOperationException("Не удалось загрузить Styles/AppLabels.xaml", ex);
            }

            // 2. Пытаемся загрузить кастомные тексты из внешнего файла
            string customLabelsPath = Path.Combine(CurrentDirectory, "Config", "AppLabels.xaml");

            if (File.Exists(customLabelsPath))
            {
                try
                {
                    using (var stream = File.OpenRead(customLabelsPath))
                    {
                        var customLabels = (ResourceDictionary)XamlReader.Load(stream);
                        this.Resources.MergedDictionaries.Add(customLabels);
                    }
                }
                catch (Exception ex)
                {
                    // Не критично - используем дефолтные тексты
                    // Логирование будет позже, когда Logger инициализируется
                    Console.WriteLine($"Предупреждение: не удалось загрузить Config/AppLabels.xaml - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Загрузка настроек разметки приложения
        /// 1. Загружаем встроенный Styles/AppLayouts.xaml (дефолтные настройки разметки)
        /// 2. Пытаемся загрузить внешний Config/AppLayouts.xaml (опциональный override)
        /// </summary>
        private void LoadAppLayouts()
        {
            // 1. Загружаем дефолтные настройки разметки из встроенного ресурса
            try
            {
                var defaultLayoutsUri = new Uri("pack://application:,,,/Styles/AppLayouts.xaml", UriKind.Absolute);
                var defaultLayouts = new ResourceDictionary { Source = defaultLayoutsUri };
                this.Resources.MergedDictionaries.Add(defaultLayouts);
            }
            catch (Exception ex)
            {
                // Критичная ошибка - дефолтные настройки разметки должны быть всегда
                throw new InvalidOperationException("Не удалось загрузить Styles/AppLayouts.xaml", ex);
            }

            // 2. Пытаемся загрузить кастомные настройки разметки из внешнего файла
            string customLayoutsPath = Path.Combine(CurrentDirectory, "Config", "AppLayouts.xaml");

            if (File.Exists(customLayoutsPath))
            {
                try
                {
                    using (var stream = File.OpenRead(customLayoutsPath))
                    {
                        var customLayouts = (ResourceDictionary)XamlReader.Load(stream);
                        this.Resources.MergedDictionaries.Add(customLayouts);
                    }
                }
                catch (Exception ex)
                {
                    // Не критично - используем дефолтные настройки разметки
                    // Логирование будет позже, когда Logger инициализируется
                    Console.WriteLine($"Предупреждение: не удалось загрузить Config/AppLayouts.xaml - {ex.Message}");
                }
            }
        }
    }
}
