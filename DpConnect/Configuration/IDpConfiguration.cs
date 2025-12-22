using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpConnect.Configuration
{
    public interface IDpConfiguration
    {
        string PropertyName { get; set; }

        IDpSourceConfiguration SourceConfiguration { get; set; }
    }
}
