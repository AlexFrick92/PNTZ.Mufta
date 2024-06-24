using PNTZ.Mufta.Launcher.GUI;
using System.Windows;

namespace PNTZ.Mufta.Launcher
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();

            MainWindow mainWin = new MainWindow();

            app.Run(mainWin);

        }
    }
}
