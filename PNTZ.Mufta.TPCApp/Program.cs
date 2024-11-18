using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            App app = new App();
            app.Start();
        }
    }
}
