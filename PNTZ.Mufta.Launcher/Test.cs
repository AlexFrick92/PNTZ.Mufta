using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.Launcher
{
    internal class Test
    {
        static void Main1(string[] args)
        {
            List<object> list = new List<object>();

            list.Add(new Data<int>());
            list.Add(new Data<string>());

            Console.WriteLine(list[0].GetType());
            Console.WriteLine(list[1].GetType());
        }

        class Data<T>
        {

        }
    }
}
