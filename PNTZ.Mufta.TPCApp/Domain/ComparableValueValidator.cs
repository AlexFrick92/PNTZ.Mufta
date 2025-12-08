using System;

using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.Domain
{
    internal class ComparableValueValidator<T> where T : IComparable<T>
    {
        public ComparableValueValidator(T min, T max, T def, string paramName) 
        {
            MaxValue = max;
            MinValue = min;
            DefaultValue = def;
            ActualValue = def;
            ParamName = paramName;
        }

        string ParamName { get; set; }

        public T DefaultValue { get; set; }

        public T MaxValue { get; set; }

        public T MinValue { get; set; }

        T actualValue;


        /// <summary>
        /// Сравнивает текущее значение с другим значением.
        /// -1 если меньше, 0 если равно, 1 если больше.
        /// </summary>
        public T ActualValue
        {
            get
            {
                return actualValue;
            }
            set
            {
                if (value.CompareTo(MaxValue) == 1)
                {
                    actualValue = MaxValue;
                    //AppInstance.Logger.Info($"Максимальное значение для параметра \"{ParamName}\" - {MaxValue}");
                }
                else if (value.CompareTo(MinValue) == -1)
                {
                    actualValue = MinValue;
                    //AppInstance.Logger.Info($"Минимальное значение для \"{ParamName}\" - {MinValue}");
                }
                else
                    actualValue = value;
            }
        }
    }

}
