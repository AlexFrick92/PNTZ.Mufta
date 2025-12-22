using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpConnect.Configuration
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DpConfigPropertyAttribute : Attribute
    {
        public string DisplayName { get; }

        public DpConfigPropertyAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
