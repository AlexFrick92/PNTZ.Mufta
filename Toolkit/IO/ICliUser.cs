using System;
using System.Collections.Generic;

namespace Toolkit.IO
{
    public interface ICliUser
    {
        event EventHandler<string> NewLineAdded;
        void EnterInput(string text);

        IEnumerable<string> ReadAndCloseBuffer();
    }
}
