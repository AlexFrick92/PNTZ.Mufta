using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestDelegates
{
    public interface IDataMethodSource
    {
        CallLowerDelegate CallLower { get; set; }
    }

    public delegate IList<object> CallLowerDelegate(params object[] args);
    public class DataMethod<I, O> : IDataMethodSource, IDataMethod where O : new()
    {
        private Dictionary<string, int> inputPropertyOrder;
        private Dictionary<string, int> outputPropertyOrder;

        public DataMethod()
        {
            inputPropertyOrder = new Dictionary<string, int>();
            outputPropertyOrder = new Dictionary<string, int>();
        }

        public void AddPropertyOrderInput(string propertyName, int order)
        {
            inputPropertyOrder[propertyName] = order;
        }

        public void AddPropertyOrderOutput(string propertyName, int order)
        {
            outputPropertyOrder[propertyName] = order;
        }

        public O Call(I arg)
        {
            return CallMethod(PrepareArgs(arg));
        }

        private List<object> PrepareArgs(I arg)
        {
            List<object> args = new List<object>();

            if (arg != null)
            {
                var orderedProperties = inputPropertyOrder.OrderBy(pair => pair.Value);

                foreach (var property in orderedProperties)
                {
                    var propertyInfo = typeof(I).GetProperty(property.Key);
                    var value = propertyInfo.GetValue(arg);
                    args.Add(value);
                }
            }
            return args;
        }
            
        private O CallMethod(List<object> mArgs)
        {
            IList<object> result;

            if (mArgs.Count > 0)
                result = CallLower(mArgs.ToArray());
            else
                result = CallLower();

            return GetOrderedResult(result);

        }

        private O GetOrderedResult(IList<object> result)
        {
            Console.WriteLine($"Тип выходного параметра: {typeof(O)}");

            if (typeof(O).IsPrimitive)
            {                
                return (O)result[0];
            }
            else
            {
                Console.WriteLine("Сложный тип");
                O orderedResult = new O();

                var orderedProperties = outputPropertyOrder.OrderBy(pair => pair.Value);

                int i = 0;
                foreach(var property in orderedProperties)
                {
                    var propertyInfo = typeof(O).GetProperty(property.Key);
                    propertyInfo.SetValue(orderedResult, result[i]);
                    i++;
                }

                return orderedResult;
            }
        }

        public CallLowerDelegate CallLower { get; set; }
        
    }

}
