
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DpConnect.Connection;

namespace DpConnect.Building
{
    internal class DpAction<T> : IDpAction<T>, IDpActionSource where T : Delegate
    {
        public SourceDelegate SourceDelegate { get; set; }

        public T Call { get; private set; }
        public bool IsConnected { get; set; }
        public event EventHandler<EventArgs> StatusChanged;

        public DpAction()
        {
            Call = CreateDelegate();
        }


        Type delegateReturnType;


        private T CreateDelegate()
        {            
            MethodInfo sourceMethod = typeof(DpAction<T>).GetMethod(nameof(SourceMethod));            
            // Получаем параметры, ожидаемые делегатом T

            ParameterInfo[] delegateParameters = typeof(T).GetMethod("Invoke").GetParameters();
            delegateReturnType = typeof(T).GetMethod("Invoke").ReturnType;

            // Создаем массив параметров Expression для лямбда-выражения
            ParameterExpression[] parameters = delegateParameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            // Создаем массив аргументов, которые будут переданы в SourceDelegate
            var argsArray = Expression.NewArrayInit(
                typeof(object),
                parameters.Select(p => Expression.Convert(p, typeof(object)))
            );

            // Создаем вызов метода SourceDelegate
            var body = Expression.Call(
                Expression.Constant(this),
                sourceMethod,
                argsArray
            );

            // Преобразуем возвращаемый результат (если возвращаемый тип делегата не void)
            var convertedBody = delegateReturnType == typeof(void) 
                ? (Expression)body                
                : Expression.Convert(body, delegateReturnType);

            // Создаем лямбда-выражение для делегата T
            return Expression.Lambda<T>(convertedBody, parameters).Compile();
        }
        public object SourceMethod(params object[] args)
        {
            IList<object> result;
            if (args.Length == 1)
            {
                Type argType = args[0].GetType();
                if (argType.IsValueType)
                {
                    result = SourceDelegate(args);
                }
                else if (argType.IsClass)
                {
                    result = SourceDelegate(PrepareArg(args[0]));
                }
                else
                    throw new NotImplementedException($"Входной аргумент {argType} не поддерживается");                            
            }
            else
            {
                foreach (var arg in args)
                    if (!arg.GetType().IsValueType)
                        throw new NotImplementedException("При использовании нескольких входных аргументов, каждый должен быть значимым типом");

                result = SourceDelegate(args);
            }            

            if (delegateReturnType == typeof(void))
                return result;
            else if (delegateReturnType.IsValueType)
            {                
                return result[0];
            }
            else if (delegateReturnType.IsClass)
            {
                var res = BoxResultToObject(result);              
                return res;
            }
            else
                throw new NotImplementedException($"Возвращаемый тип {delegateReturnType} не поддерживается");
        }

        object[] PrepareArg(object arg)
        {            
            List<object> preparedArgs = new List<object>();

            foreach (var property in arg.GetType().GetProperties())
            {                
                preparedArgs.Add(property.GetValue(arg));
            }            
            return preparedArgs.ToArray();
        }

        object BoxResultToObject(IList<object> listOfResult)
        {
            object objectResult = Activator.CreateInstance(delegateReturnType);

            int i = 0;
            
            foreach (var property in delegateReturnType.GetProperties())
            {               

                property.SetValue(objectResult, listOfResult[i]);
                i++;
            }            

            return objectResult;
        }

    }
}
