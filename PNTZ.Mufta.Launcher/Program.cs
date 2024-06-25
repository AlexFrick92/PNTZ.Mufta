using PNTZ.Mufta.Launcher.View;
using PNTZ.Mufta.Launcher.ViewModel;
using System.Windows;
using Promatis.DebuggingToolkit.IO;

namespace PNTZ.Mufta.Launcher
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();



            Cli cli = new Cli();

            cli.RegisterCommand("print", (args) => (cli as ICliProgram).WriteLine(args[0]));


            cli.EnterInput("Hello!");

            MainViewModel mainViewModel = new MainViewModel(cli);
            MainView mainWin = new MainView(mainViewModel);
            


            app.Run(mainWin);

        }
    }
}
