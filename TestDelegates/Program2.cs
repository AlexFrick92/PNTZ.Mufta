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

            dataMethod.AddPropertyOrderInput("Op1", 0);
            dataMethod.AddPropertyOrderInput("Op2", 1);

            dataMethod.AddPropertyOrderOutput("Result", 0);

            Console.WriteLine("Создаем провайдера и запускаем");
            ProviderDummy providerDummy = new ProviderDummy();
            providerDummy.Start();

            Console.WriteLine("Регистрируем метод в провайдере");
            providerDummy.RegisterMethod("ns=3;s=\"MathOperation\"",dataMethod);

            Console.WriteLine("Вызов метода");

            DmInputParams dmInputParams = new DmInputParams() { Op1 = 2, Op2 = 2};

            var result = dataMethod.Call(dmInputParams);

            Console.WriteLine("Метод выполнился с результатом: " + result.Result);



            return;

            Console.WriteLine("Пробный вызов метода");
            providerDummy.CallSimpleMethod();
        }
    }
}
