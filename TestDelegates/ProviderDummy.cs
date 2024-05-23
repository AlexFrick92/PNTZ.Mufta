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

        public void RegisterMethod(DataMethod method)
        {
            method.CallLower += CallMethod;
        }

        private List<object> CallMethod(params object[] args)
        {
            Console.WriteLine("Входные аргументы:");

            foreach (object o in args) 
                Console.WriteLine(o.ToString() + " : " + o.GetType());

            return new List<object>();
        }
    }
}
