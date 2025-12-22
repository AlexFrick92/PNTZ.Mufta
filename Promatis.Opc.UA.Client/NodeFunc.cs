using Opc.Ua;

namespace Promatis.Opc.UA.Client
{
    /// <summary>
    /// Нода для вызова функции
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    public class NodeFunc<T> : NodeBase<T>
    {
        public NodeId NodeMethodId { get; }
        public NodeFunc(string nodeIds):base(nodeIds)
        {
            NodeMethodId = $"{nodeIds}.Method";
        }
    }
}