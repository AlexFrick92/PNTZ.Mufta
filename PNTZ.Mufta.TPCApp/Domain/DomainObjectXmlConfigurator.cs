using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PNTZ.Mufta.TPCApp.Domain
{
    internal class DomainObjectXmlConfigurator<T>
    {
        XDocument config;

        public DomainObjectXmlConfigurator()
        {
            config = XDocument.Load($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Domain/{typeof(T).Name}.xml");

            InitValidators();
        }
        public T1 GetFloatValue<T1>(string propName) where T1 : IComparable<T1> => ((ComparableValueValidator<T1>)Validators[propName]).ActualValue;
        public void SetFloatValue<T1>(string propName, T1 value) where T1 : IComparable<T1> => ((ComparableValueValidator<T1>)Validators[propName]).ActualValue = value;

        Dictionary<string, object> Validators = new Dictionary<string, object>();
        public T1 GetValueFromXml<T1>(string propName, string attribute)
        {
            string xmlVal = config.Root.Element(propName).Attribute(attribute).Value;
            return (T1)Convert.ChangeType(xmlVal, typeof(T1), CultureInfo.InvariantCulture);
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
