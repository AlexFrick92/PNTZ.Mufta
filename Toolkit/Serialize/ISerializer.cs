using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Serialize
{
    public interface ISerializer
    {
        void Deserialize(SerializableBase obj);
        void Serialize(SerializableBase obj);       
    }
}
