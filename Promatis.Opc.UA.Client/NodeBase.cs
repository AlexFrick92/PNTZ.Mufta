using System;
using System.Collections.Generic;
using NLog.Filters;
using Opc.Ua;
using Opc.Ua.Client;

namespace Promatis.Opc.UA.Client
{
    public abstract class NodeBase<T>
    {
        /// <summary>
        /// Id Ноды
        /// </summary>
        public NodeId NodeId { get; }
        /// <summary>
        /// Значение ноды
        /// </summary>
        public T Value { get; set; }

        public DateTime Time { get; set; }
        protected NodeBase(string nodeIds)
        {
            NodeId = new NodeId(nodeIds);
            //Value = new T();            
        }
        public void FromNode(IList<object> dataValue)=>  Value = Value is INodeConverter<T> v ? v.FromNode(dataValue) : (T) dataValue[0];
        

        public void FromNode(DataValue dataValue)
        {
            Time = dataValue.SourceTimestamp;
            var extention = Value as ComplexType;
                   
            if(typeof(T).IsSubclassOf(typeof(ComplexType)))            
            {
                Value = (T)Activator.CreateInstance(typeof(T));
                //Здесь мы создаём инстанрс ComplexType у которого есть конструктор без параметров
                //При этом, ограничение T: new() нам не нужно, потому что тогда мы не сможем создать NodeValue<string>                

                ComplexType complex = Value as ComplexType;
                complex.Value = (ExtensionObject)dataValue.Value;
            }
            else
            {
                Value = Value is INodeConverter<T> ? default : (T)dataValue.Value;
            }
        }
    }
}