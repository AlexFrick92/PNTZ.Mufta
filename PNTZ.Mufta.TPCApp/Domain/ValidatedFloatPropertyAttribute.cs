using System;

namespace PNTZ.Mufta.TPCApp.Domain
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidatedFloatPropertyAttribute : Attribute
    {
        public string Name { get; }

        public ValidatedFloatPropertyAttribute(string xmlConfigName)
        {
            Name = xmlConfigName;
        }
    }
}
