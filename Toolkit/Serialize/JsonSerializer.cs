using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Serialize
{
    public class JsonSerializer<T> : ISerializer where T : SerializableBase
    {
        string _path;
        public JsonSerializer(string path)
        {
            _path = path;
        }
        public void Deserialize(SerializableBase obj)
        {
            using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                throw new NotImplementedException("Требуется реализовать сериализатор!");
                //T person = System.Text.Json.JsonSerializer.Deserialize<T>(fs);
            }
        }

        public void Serialize(SerializableBase obj)
        {
            throw new NotImplementedException();
        }
    }
}
