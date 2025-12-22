using System.Reflection;
using Opc.Ua;

namespace Promatis.Opc.UA.Client
{
    public class Atributes
    {

        /// <summary>
        /// Returns the id for the attribute with the specified browse name.
        /// </summary>
        public static uint GetIdentifier(string browseName)
        {
            FieldInfo[] fields = typeof(Attributes).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                if (field.Name == browseName)
                {
                    return (uint)field.GetValue(typeof(Attributes));
                }
            }

            return 0;
        }

    }
}