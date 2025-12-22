using System;
using Opc.Ua.Client;

namespace Promatis.Opc.UA.Client
{
    /// <summary>
    /// Нода с OPC UA типизированная
    /// </summary>
    /// <typeparam name="T">Тип значения ноды</typeparam>
    public class NodeValue<T> :NodeBase<T>
    {
        /// <summary>
        /// Событие вызываемое при изменении значения в ноде
        /// </summary>
        public event EventHandler<T> OnChange;

        public NodeValue(string nodeIds):base(nodeIds)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeIds">ID ноды</param>
        /// <param name="onChange">Событие, которые вызывается при изменении значения ноды</param>
        public NodeValue(string nodeIds, EventHandler<T> onChange) : this(nodeIds)
        {
            OnChange = onChange;
        }

        
        public void ItemNotification(MonitoredItem monitoreditem, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in monitoreditem.DequeueValues())
            {
                FromNode(value);
            }
            OnChange?.Invoke(this, Value);
        }
        
    }
}