using Newtonsoft.Json.Linq;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDelegates
{
    internal class ProgramTestObject
    {
        public static void Main(string[] args)
        {
            CheckedDataMethod dataMethod = new CheckedDataMethod();            

            dataMethod.InputArgs = new List<Type>() { typeof(int), typeof(double) };
            dataMethod.OutputArgs = new List<Type>() { typeof(int), typeof(double) };

            List<object> result = dataMethod.Call(1, 2.2);
            foreach (object obj in result)
            {
                Console.WriteLine(obj.GetType());
            }

        }

    }
    public class CheckedDataMethod
    {
        public List<Type> InputArgs;
        public List<Type> OutputArgs;


        public List<object> Call(params object[] args)
        {
            
            for (int i = 0; i < InputArgs.Count; i++)
            {
                if (args[i].GetType() != InputArgs[i])
                    throw new Exception("Несоответствие типов");

            }            

            var result = new List<object>();
            for (int i = 0; i < OutputArgs.Count; i++)
            {
                result.Add(Activator.CreateInstance(OutputArgs[i]));
            }
            return result;
        }
    }

}
