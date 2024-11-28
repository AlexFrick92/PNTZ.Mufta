using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp
{
    internal class Program
    {
        // Здесь будет вызван класс в UI потоке
        [STAThread]
        static void Main(string[] args)
        {
            App app = new App();
            app.Start();
        }
    }
}
