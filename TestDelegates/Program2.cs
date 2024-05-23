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
            DataMethod dataMethod = new DataMethod() { Greeting = "Привет!", Number = 4 };

            Console.WriteLine("Создаем провайдера");
            ProviderDummy providerDummy = new ProviderDummy();

            Console.WriteLine("Регистрируем метод в провайдере");
            providerDummy.RegisterMethod(dataMethod);

            Console.WriteLine("Вызов метода");
            dataMethod.Call();
        }
    }
}
