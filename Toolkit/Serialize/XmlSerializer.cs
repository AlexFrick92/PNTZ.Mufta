using System;
using System.Xml.Linq;

namespace Toolkit.Serialize
{
    public class XmlSerializer : ISerializer
    {        
        public XmlSerializer(string path)
        {
            _path = path;
        }
        string _path;
        public void Deserialize(SerializableBase obj)
        {
            var root = XDocument.Load(_path).Root;                  

            foreach (var xmlProp in root.Elements())
            {                
                var property = obj.GetType().GetProperty(xmlProp.Name.ToString());
                property.SetValue(obj, Convert.ChangeType(xmlProp.Value, property.PropertyType));
            }            
        }

        public void Serialize(SerializableBase obj)
        {
            throw new NotImplementedException();
        }        
    }
}
