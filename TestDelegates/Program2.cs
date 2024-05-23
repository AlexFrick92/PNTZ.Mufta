using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDelegates
{
    internal class Program2
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Создаём метод");
            DataMethod<DmInputParams, DmOutputParams> dataMethod = new DataMethod<DmInputParams, DmOutputParams>();

            dataMethod.AddPropertyOrderInput("Greeting", 0);
            dataMethod.AddPropertyOrderInput("Number", 1);

            dataMethod.AddPropertyOrderOutput("ReturnedString", 0);

            Console.WriteLine("Создаем провайдера");
            ProviderDummy providerDummy = new ProviderDummy();

            Console.WriteLine("Регистрируем метод в провайдере");
            providerDummy.RegisterMethod(dataMethod);

            Console.WriteLine("Вызов метода");

            DmInputParams dmInputParams = new DmInputParams() { Greeting = "Привет!", Number = 2};


            var result = dataMethod.Call(dmInputParams);

            Console.WriteLine("Метод выполнился с результатом: " + result.ReturnedString);



        }
    }
}
