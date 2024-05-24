using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestDelegates
{
    internal class Program2
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Создаём метод");

            XDocument dmConfig = XDocument.Load("DmConfiguration.xml");
            XElement methodConfig = dmConfig.Element("DmDefinition").Element("Method");
            XElement inputParamConfig = methodConfig.Element("InputParameters");
            XElement outputParamConfig = methodConfig.Element("OutputParameters");

            string configInputType = inputParamConfig.Attribute("Type").Value;
            string configOutputType = outputParamConfig.Attribute("Type").Value;            

            Type inputType = Type.GetType(configInputType);
            Type outputType = Type.GetType(configOutputType);

            var dmType = typeof(DataMethod<,>).MakeGenericType(inputType, outputType);
            IDataMethod method = (IDataMethod)Activator.CreateInstance(dmType);

            foreach(var parameter in inputParamConfig.Elements("Parameter"))
            {
                method.AddPropertyOrderInput(parameter.Attribute("Name").Value, int.Parse(parameter.Attribute("Order").Value));
            }

            foreach (var parameter in outputParamConfig.Elements("Parameter"))
            {
                method.AddPropertyOrderOutput(parameter.Attribute("Name").Value, int.Parse(parameter.Attribute("Order").Value));
            }         

            Console.WriteLine("Создаем провайдера и запускаем");
            ProviderDummy providerDummy = new ProviderDummy();
            providerDummy.Start();

            Console.WriteLine("Регистрируем метод в провайдере");
            providerDummy.RegisterMethod("ns=3;s=\"MathOperation\"",(IDataMethodSource)method);

            Console.WriteLine("Вызов метода");

            MyDmInputParams dmInputParams = new MyDmInputParams() { Op1 = 2, Op2 = 2};

            var result = ((DataMethod<MyDmInputParams,MyDmOutputParams>)method).Call(dmInputParams);

            Console.WriteLine("Метод выполнился с результатом: " + result.Result);



            return;

            Console.WriteLine("Пробный вызов метода");
            providerDummy.CallSimpleMethod();
        }
    }
}
