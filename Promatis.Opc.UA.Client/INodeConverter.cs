using System.Collections.Generic;

namespace Promatis.Opc.UA.Client
{
    /// <summary>
    /// Интерфейс, который должен реализовывать  тип для формирования из ноды
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INodeConverter<out T>
    {
        T FromNode(IList<object> dataValue);
    }
}