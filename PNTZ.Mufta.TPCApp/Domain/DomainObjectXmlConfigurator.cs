using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.Domain
{
    internal class DomainObjectXmlConfigurator<T>
    {
        XDocument config;

        public DomainObjectXmlConfigurator()
        {
            config = XDocument.Load($"{AppInstance.CurrentDirectory}/Domain/{typeof(T).Name}.xml");

            InitValidators();
        }
        public T GetFloatValue<T>(string propName) where T : IComparable<T> => ((ComparableValueValidator<T>)Validators[propName]).ActualValue;
        public void SetFloatValue<T>(string propName, T value) where T : IComparable<T> => ((ComparableValueValidator<T>)Validators[propName]).ActualValue = value;

        Dictionary<string, object> Validators = new Dictionary<string, object>();
        public T GetValueFromXml<T>(string propName, string attribute)
        {
            string xmlVal = config.Root.Element(propName).Attribute(attribute).Value;
            return (T)Convert.ChangeType(xmlVal, typeof(T));
        }

        void InitValidators()
        {
            var props = typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(ComparableValidationPropertyAttribute)));

            foreach (var prop in props)
            {
                ComparableValidationPropertyAttribute attribute = (ComparableValidationPropertyAttribute)Attribute.GetCustomAttribute(prop, typeof(ComparableValidationPropertyAttribute));

                string xmlname = attribute.XmlConfigName;

                MethodInfo getXmlVal = GetType().GetMethod(nameof(GetValueFromXml)).MakeGenericMethod(prop.PropertyType);

                dynamic min = getXmlVal.Invoke(this, new object[] { xmlname, "Min" });
                dynamic max = getXmlVal.Invoke(this, new object[] { xmlname, "Max" });
                dynamic def = getXmlVal.Invoke(this, new object[] { xmlname, "Default" });

                string propName = GetValueFromXml<string>(xmlname, "Name");

                Type specifiedValidator = typeof(ComparableValueValidator<>).MakeGenericType(prop.PropertyType);
                object validator = Activator.CreateInstance(specifiedValidator, min, max, def, propName);

                Validators.Add(prop.Name, validator);
            }
        }

    }
}
