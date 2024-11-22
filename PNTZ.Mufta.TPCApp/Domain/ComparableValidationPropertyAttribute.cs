using System;

namespace PNTZ.Mufta.TPCApp.Domain
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComparableValidationPropertyAttribute : Attribute
    {
        public string XmlConfigName { get; }

        public ComparableValidationPropertyAttribute(string xmlConfigName)
        {
            XmlConfigName = xmlConfigName;
        }
    }
}
