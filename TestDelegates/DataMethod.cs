using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDelegates
{
    public delegate List<object> CallLowerDelegate(params object[] args);

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OrderAttribute : Attribute
    {
        public int Order { get; }
        public OrderAttribute(int order)
        {
            Order = order;
        }
    }

    public class DataMethod
    {
        [Order(0)]
        public string Greeting { get; set; }

        [Order(1)]
        public int Number { get; set; }


        public void Call()
        {
            CallMethod(PrepareArgs(this));
        }

        private List<object> PrepareArgs(DataMethod obj)
        {
            List<object> args = new List<object>();

            if (obj != null)
            {
                var properties = obj.GetType().GetProperties()
                    .Where(p => p.GetCustomAttribute<OrderAttribute>() != null)
                    .OrderBy(p => p.GetCustomAttribute<OrderAttribute>().Order);

                foreach (var property in properties)
                {
                    var value = property.GetValue(obj);
                    args.Add(value);
                }
            }
            return args;
        }
            
        private void CallMethod(List<object> mArgs)
        {
            if (mArgs.Count > 0)
                CallLower(mArgs.ToArray());
            else
                CallLower();
        }

        public CallLowerDelegate CallLower;
        
    }
}
