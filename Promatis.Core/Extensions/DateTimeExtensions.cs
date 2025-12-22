using System;
using System.Globalization;

namespace Promatis.Core.Extensions
{
    /// <summary>
    /// Методы расширения для типа <see cref="DateTime"/>
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Время
        /// </summary>
        public static DateTime Time(this DateTime dateTime) => DateTime.MinValue.Date.Add(dateTime.TimeOfDay);

        /// <summary>
        /// Дата
        /// </summary>
        public static DateTime Date(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

        /// <summary>
        /// Дата начала по умолчанию
        /// </summary>
        public static DateTime BeginDate(this DateTime dateTime) => DateTime.MinValue.Date;

        /// <summary>
        /// Дата окончания по умолчанию
        /// </summary>
        public static DateTime EndDate(this DateTime dateTime) => DateTime.MinValue.Date;

        /// <summary>
        /// Дата начала дня
        /// </summary>
        public static DateTime BeginOfDay(this DateTime dateTime) => dateTime.Date;

        /// <summary>
        /// Дата окончания дня
        /// </summary>
        public static DateTime EndOfDay(this DateTime dateTime) => dateTime.Date.AddDays(1).AddSeconds(-1);

        /// <summary>
        /// Дата начала недели
        /// </summary>
        public static DateTime FirstDayOfWeek(this DateTime dateTime)
        {
            var dayRu = dateTime.DayOfWeek == 0 ? 7 : (int) dateTime.DayOfWeek;
            var delta = 1 - dayRu;
            return dateTime.AddDays(delta);
        }

        /// <summary>
        /// Дата окончания недели
        /// </summary>
        public static DateTime LastDayOfWeek(this DateTime dateTime)
        {
            var dayRu = dateTime.DayOfWeek == 0 ? 7 : (int) dateTime.DayOfWeek;
            var delta = 7 - dayRu;
            return dateTime.AddDays(delta);
        }

        /// <summary>
        /// Дата начала месяца
        /// </summary>
        public static DateTime FirstDayOfMonth(this DateTime dateTime) =>
            new DateTime(dateTime.Year, dateTime.Month, 1);

        /// <summary>
        /// Дата окончания месяца
        /// </summary>
        public static DateTime LastDayOfMonth(this DateTime dateTime) =>
            dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        /// <summary>
        /// Дата начала года
        /// </summary>
        public static DateTime FirstDayOfYear(this DateTime dateTime) => new DateTime(dateTime.Year, 1, 1);

        /// <summary>
        /// Дата окончания года
        /// </summary>
        public static DateTime LastDayOfYear(this DateTime dateTime) =>
            dateTime.FirstDayOfYear().AddYears(1).AddDays(-1);

        /// <summary>
        /// Признак , показывающий что указанная дата является выходным днем (суббота или воскресенье)
        /// </summary>
        public static bool IsWeekEnd(this DateTime dateTime) =>
            dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        /// <summary>
        /// Имя месяца с большой буквы
        /// </summary>
        public static string MonthName(this DateTime dateTime) => GetMonthName(dateTime.Month);

        /// <summary>
        /// Имя месяца в родительном падеже с маленькой буквы
        /// </summary>
        public static string GenetiveMonthName(this DateTime dateTime) => GetGenetiveMonthName(dateTime.Month);

        /// <summary>
        /// Имя дня недели
        /// </summary>
        public static string DayOfWeekName(this DateTime dateTime)
        {
            var no = (int) dateTime.DayOfWeek;
            return no >= 0 && no <= 6
                ? GetDateTimeFormatInfo().GetShortestDayName(dateTime.DayOfWeek).ToLower()
                : string.Empty;
        }

        /// <summary>
        /// Переводит UTC в локальную дату
        /// </summary>
        public static DateTime UtcToLocal(this DateTime dateTime) =>
            new DateTime(dateTime.Ticks, DateTimeKind.Utc).ToLocalTime();

        /// <summary>
        /// Определяет меньшую дату из двух
        /// </summary>
        /// <param name="dateTime">Текущее значение</param>
        /// <param name="value">Сравниваемая дата</param>
        /// <returns>Значение меньшей даты</returns>
        public static DateTime MinOf(this DateTime dateTime, DateTime value) => dateTime < value ? dateTime : value;

        /// <summary>
        /// Определяет большую дату из двух
        /// </summary>
        /// <param name="dateTime">Текущее значение</param>
        /// <param name="value">Сравниваемая дата</param>
        /// <returns>Значение меньшей даты</returns>
        public static DateTime Max(this DateTime dateTime, DateTime value) => dateTime > value ? dateTime : value;

        /// <summary>
        /// Округляет текущее значение до указанной точности
        /// </summary>
        /// <param name="datetime">Текущее значение</param>
        /// <param name="roundingInterval">Точность</param>
        /// <returns>Округленная дата</returns>
        public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval) =>
            new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);

        /// <summary>
        /// Округляет текущее значение до указанной точности
        /// </summary>
        /// <param name="timeSpan">Текущее значение</param>
        /// <param name="roundingInterval">Точность</param>
        /// <returns>Округленный временной интервал</returns>
        public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan roundingInterval) =>
            Round(timeSpan, roundingInterval, MidpointRounding.ToEven);

        /// <summary>
        /// Округляет текущее значение до указанной точности
        /// </summary>
        /// <param name="timeSpan">Текущее значение</param>
        /// <param name="roundingInterval">Точность</param>
        /// <param name="roundingType">Тип округления</param>
        /// <returns>Округленный временной интервал</returns>
        public static TimeSpan Round(this TimeSpan timeSpan, TimeSpan roundingInterval, MidpointRounding roundingType)
        {
            return new TimeSpan(
                Convert.ToInt64(Math.Round(
                    timeSpan.Ticks / (decimal) roundingInterval.Ticks,
                    roundingType
                )) * roundingInterval.Ticks
            );
        }

        /// <summary>
        /// Получает название месяца с большой буквы
        /// </summary>
        /// <param name="monthNum">Номер месяца от 1 до 12</param>
        /// <returns>Название месяца с большой буквы. Если номер месяца не входит в допустимый диапазон, то возвращает пустую строку</returns>
        private static string GetMonthName(int monthNum) => monthNum < 1 || monthNum > 12
            ? string.Empty
            : GetDateTimeFormatInfo().GetMonthName(monthNum);

        /// <summary>
        /// Получает имя месяца в родительном падеже с маленькой буквы
        /// </summary>
        /// <param name="monthNum">Номер месяца от 1 до 12</param>
        /// <returns>Имя месяца в родительном падеже с маленькой буквы. Если номер месяца не входит в допустимый диапазон, то возвращает пустую строку</returns>
        private static string GetGenetiveMonthName(int monthNum) => monthNum < 1 || monthNum > 12
            ? string.Empty
            : GetDateTimeFormatInfo().MonthGenitiveNames[monthNum - 1].ToLower();

        /// <summary>
        /// Получает объект DateTimeFormatInfo в соответствии с текущей культурой
        /// </summary>
        /// <returns></returns>
        private static DateTimeFormatInfo GetDateTimeFormatInfo() =>
            new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
    }
}
