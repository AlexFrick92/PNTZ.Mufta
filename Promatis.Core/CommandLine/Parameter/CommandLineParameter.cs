using System;
using System.Reflection;
using Promatis.Core.Extensions;

namespace Promatis.Core.CommandLine
{
    /// <summary>
    /// ѕараметр командной строки
    /// </summary>
    internal class CommandLineParameter
    {
        private readonly PropertyInfo _property;
        private readonly CommandLineParameterAttribute _attribute;
        private bool _isValueSetted;

        /// <summary>
        /// »нициализирует новый экземпл€р <see cref="CommandLineParameter"/>
        /// </summary>
        /// <param name="property">—войство аргумента, дл€ которого установлен атрибут <see cref="CommandLineParameterAttribute"/></param>
        /// <param name="attribute">јтрибут, заданный дл€ свойства</param>
        /// <exception cref="ArgumentNullException">ƒл€ всех параметров</exception>
        internal CommandLineParameter(PropertyInfo property, CommandLineParameterAttribute attribute)
        {
            Guard.IsNotNull(property, "property");
            Guard.IsNotNull(attribute, "attribute");

            _property = property;
            _attribute = attribute;
        }

        /// <summary>
        /// ѕризнак того что значение параметра установлено дл€ аргумента
        /// </summary>
        internal bool ArgumentSupplied => !_attribute.Required || _isValueSetted;

        /// <summary>
        ///  люч-значение, которое соответствует наименованию параметра в аргументах командной строки
        /// </summary>
        internal string Key => _attribute.IsArg ? CommandLineEnvironment.GetParameterKey(_attribute.Position) :
            CommandLineEnvironment.CaseSensitive ? _attribute.Key : _attribute.Key.ToLowerInvariant();

        /// <summary>
        /// ѕозици€ параметра
        /// </summary>
        internal int Position => _attribute.Position;

        /// <summary>
        /// ѕризнак, указывающий на то, что знаение параметра валидно. 
        /// </summary>
        internal bool IsValidValue => !_attribute.Required || _isValueSetted;

        /// <summary>
        /// ”станавливает значение по умолчанию дл€ свойства экземпл€ра аргумента, которое имеет этот атрибут
        /// </summary>
        /// <param name="argument">Ёкземпл€р аргумента</param>
        internal void SetDefaultValue(object argument)
        {
            if (_attribute.DefaultValue != null)
            {
                var property = argument.GetType().GetProperty(_property.Name);
                property?.SetValue(argument, _attribute.DefaultValue, null);
            }
        }

        /// <summary>
        /// ”станавливает значение дл€ свойства экземпл€ра аргумента, которое имеет этот атрибут
        /// </summary>
        /// <param name="argument">Ёкземпл€р аргумента</param>
        /// <param name="value">«начение</param>
        internal void SetValue(object argument, string value)
        {
            if (_property.PropertyType == typeof(bool))
                _property.SetValue(argument, value.IsEmpty() || Convert.ToBoolean(value), null);
            else if (_property.PropertyType == typeof(int))
                _property.SetValue(argument, Convert.ToInt32(value), null);
            else if (_property.PropertyType == typeof(long))
                _property.SetValue(argument, Convert.ToInt64(value), null);
            else if (_property.PropertyType == typeof(DateTime))
                _property.SetValue(argument, Convert.ToDateTime(value), null);
            else if (_property.PropertyType == typeof(string))
                _property.SetValue(argument, value, null);
            //else
            //    throw new CommandLineException(
            //        new CommandArgumentHelp(
            //            argument.GetType(), Strings.UnsupportedPropertyType(Property.PropertyType)));

            _isValueSetted = true;
        }
    }
}
