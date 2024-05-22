using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Cons = System.Console;


namespace Promatis.Binding.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {


            OpcUaMethod newMet = new OpcUaMethodGen<int, string>()
            {
                DoSm = (a) => (a + a + a).ToString(),
            };



            DataProc proc = new DataProc();

            //proc.RegisterMethod(a => (int)a+(int)a);

            proc.RegMet(newMet);

            proc.DoProc(2);
        }
    }

    public delegate object[] DoSm(params object[] p);

    public class OpcUaMethod
    {
        public string Name { get; set; }
    }

    public class OpcUaMethodGen<T,R> : OpcUaMethod 
    {        
        public Func<T, R> DoSm { get; set; }
    }

    public class DataProc
    {
        private Func<int, int> Operaton;

        
        public void RegMet(OpcUaMethod met)
        {
            Operaton = ((OpcUaMethodGen<int,int>)met).DoSm;
        }

        public void RegisterMethod(Func<object, object> method)
        {
            var met = Convert(method, typeof(int), typeof(int));

            Operaton = (i) => (int)met.DynamicInvoke(i);
            
        }

        public void DoProc(int param)
        {
            Cons.WriteLine(Operaton(param));
        }

        public static Delegate Convert(Func<object, object> func, Type argType, Type resultType)
        {
            // If we need more versions of func then consider using params Type as we can abstract some of the
            // conversion then.

            Contract.Requires(func != null);
            Contract.Requires(resultType != null);

            var param = Expression.Parameter(argType);
            var convertedParam = new Expression[] { Expression.Convert(param, typeof(object)) };

            // This is gnarly... If a func contains a closure, then even though its static, its first
            // param is used to carry the closure, so its as if it is not a static method, so we need
            // to check for that param and call the func with it if it has one...
            Expression call;
            call = Expression.Convert(
                func.Target == null
                ? Expression.Call(func.Method, convertedParam)
                : Expression.Call(Expression.Constant(func.Target), func.Method, convertedParam), resultType);

            var delegateType = typeof(Func<,>).MakeGenericType(argType, resultType);
            return Expression.Lambda(delegateType, call, param).Compile();
        }
    }

    
}
