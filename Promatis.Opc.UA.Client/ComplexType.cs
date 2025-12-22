using Opc.Ua;
using System;
using System.Reflection;

namespace Promatis.Opc.UA.Client
{
    public class ComplexType
    {
        public ExtensionObject Value { get; set; }

        protected T GetValue<T>(string name) => (T)Value.Body.GetType().GetProperty(name)?.GetGetMethod().Invoke(Value.Body, null);

        
        //       protected T SetValue<T>(string name, T value) => (T)Value.Body.GetType().GetProperty(name)?.GetSetMethod().Invoke(Value.Body, value);
    }

    
    public class ComplexType<T> : ComplexType
    {
        T extractedValue;
        public T ExtractedValue
        {
            get
            {
                extractedValue = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {

                    object value = GetValue(prop.Name);

                    prop.SetValue(extractedValue, value);
                }

                return extractedValue;
            }

            set
            {
                extractedValue = value;
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    //Console.WriteLine(Value.Body.GetType());
                    //Console.WriteLine(Value.Body.GetType().GetProperty(prop.Name).PropertyType);

                    //Console.WriteLine("Установим значение " + prop.GetValue(value) + ":" + prop.PropertyType);

                    SetValue(prop.Name, prop.GetValue(value));                                        
                }

            }
        }
        object GetValue(string name) => Value.Body.GetType().GetProperty(name)?.GetGetMethod().Invoke(Value.Body, null);

        void SetValue(string name, object value) => Value.Body.GetType().GetProperty(name)?.GetSetMethod().Invoke(Value.Body, new object[] { value });

    }
}