using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TestDelegates
{
    public class ProviderDummy 
    {

        public void RegisterMethod(IDataMethodSource method)
        {
            method.CallLower += CallMethod;
        }

        private List<object> CallMethod(params object[] args)
        {
            Console.WriteLine("Входные аргументы:");

            foreach (object o in args) 
                Console.WriteLine(o.ToString() + " : " + o.GetType());


            string a = args[0].ToString() + args[1].ToString();

            Console.WriteLine($"Помещаю результат: {a}");

            return new List<object>() {a};
        }
    }
}
