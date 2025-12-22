using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Serialize
{
    public abstract class SerializableBase
    {
        ISerializer _serializer;

        protected SerializableBase(ISerializer serializer)
        {
            _serializer = serializer;
            _serializer.Deserialize(this);
        }        
    }
}
