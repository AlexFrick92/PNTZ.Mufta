using System;

namespace Promatis.Opc.UA.Client
{
    /// <summary>
    /// Название поля в OPC
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OpcDataAttribute:Attribute
    {
        public string Name { get; set; }

        public OpcDataAttribute(string name)
        {
            Name = name;
        }
    }

    public enum OpcDataType
    {
        Variable,
        Array,
    }
}