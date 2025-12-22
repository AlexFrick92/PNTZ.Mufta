using Toolkit.IO;
using Desktop.Application.View;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System;


namespace Desktop.Application
{
    public abstract class StagedApplication : System.Windows.Application
    {
        private Window _greetingWindow;
        private Window _mainWindow;
        protected Window mainWindow { get { return _mainWindow; } set { _mainWindow = value; } }

        protected Cli cli;
        protected string currentDirectory;
        public StagedApplication()
        {

            cli = new Cli();
            currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                string path = currentDirectory + "/unhandledExceptions.txt";

                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine($"{DateTime.UtcNow}:{e.ExceptionObject}");
                }

                Window exc = new View.UnhandledExceptionView(e.ExceptionObject.ToString());
                exc.ShowDialog();

            };            

        }

        public void Start()
        {
            cli.RegisterCommand("exit", async (args) =>
            {
                cli.WriteLine("Exiting...");
                BeforeExit();
                await Task.Delay(5000);
                this.Shutdown();

            });

            BeforeInit();
            ShowGreeting();
            Init();
            Task.Delay(1000).Wait();
            ShowMain();
            AfterInit();
            base.Run();
        }

        protected virtual void BeforeInit() { }
        protected virtual void Init() { }
        protected virtual void AfterInit() { }
        protected virtual void BeforeExit() { }


        void ShowGreeting()
        {
            if (_greetingWindow != null)
                _greetingWindow.Show();
            else
            {
                _greetingWindow = new GreetingView();
                _greetingWindow.Show();
            }
        }
        void ShowMain()
        {
            if (_mainWindow != null)
            {
                _mainWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/Desktop;component/Images/Icon.png", UriKind.RelativeOrAbsolute));
                _mainWindow.Show();
                if (_greetingWindow != null)
                {
                    _greetingWindow.Hide();
                    _mainWindow.Closed += (s, v) => _greetingWindow.Close();
                }

            }
            else
            {
                throw new Exception("Рабочее окно не задано!");
            }
        }

    }
}
