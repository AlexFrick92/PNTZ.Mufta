using System;

namespace PNTZ.Mufta.Showcase.Models
{
    /// <summary>
    /// Информация о контроле в галерее
    /// </summary>
    public class ControlInfo
    {
        /// <summary>
        /// Название контрола
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание контрола
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Категория (например: "Графики", "Индикаторы", "Формы")
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Тип окна для тестирования контрола
        /// </summary>
        public Type WindowType { get; set; }
    }
}
