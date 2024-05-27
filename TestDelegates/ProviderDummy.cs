using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using Promatis.DpProvider.OpcUa;

namespace TestDelegates
{
    public class ProviderDummy 
    {

        public void RegisterMethod(string NodeId, IDataMethodSource method)
        {
            method.CallLower += (e) => CallMethod(NodeId, e);
        }

        OpcUaProvider _client;

        public void Start()
        {
            _client = new OpcUaProvider();
            _client.ConfigureHost(XDocument.Load("HostConfiguration.xml"));
            _client.Start();

        }

        public void CallSimpleMethod()
        {
            string NodeId = "ns=3;s=\"MathOperation\"";

           // var result = _client.(NodeId, (Single)1, (Single)1);

            //foreach (object obj in result)
            //{
            //    Console.WriteLine(obj);
            //}
        }

        private IList<object> CallMethod(string NodeId, params object[] args)
        {
            Console.WriteLine("Вызываем метод");
         //   return _client.Call(NodeId, args);            
        }
    }
}
