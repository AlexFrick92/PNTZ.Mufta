namespace Promatis.Core.Integration
{
    /// <summary>
    /// Интерфейс обработчика запроса интеграционной шины
    /// </summary>
    /// <typeparam name="TOut">Результат запроса</typeparam>
    public interface IRequestHandler<out TOut>
    {
        /// <summary>
        /// Задает метод обработки запроса и выполняет его
        /// </summary>
        /// <param name="methodName">Наименование метода</param>
        /// <returns>Результат выполнения</returns>
        TOut Process(string methodName);
    }

    /// <summary>
    /// Интерфейс обработчика параметризованного запроса интеграционной шины
    /// </summary>
    /// <typeparam name="TIn">Тип входного аргумента</typeparam>
    /// <typeparam name="TOut">Результат запроса</typeparam>
    public interface IRequestHandler<in TIn, out TOut>
    {
        /// <summary>
        /// Задает метод обработки запроса, его параметры и выполняет его
        /// </summary>
        /// <param name="methodName">Наименование метода</param>
        /// <param name="args">Аргументы запроса</param>
        /// <returns>Результат выполнения</returns>
        TOut Process(string methodName, TIn args);
    }
}
