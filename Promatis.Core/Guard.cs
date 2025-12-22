using System;
using Promatis.Core.Extensions;
using Promatis.Core.Resources;

namespace Promatis.Core
{
    /// <summary>
    /// Реализует методы по защите параметров и локальных переменных.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не унаследован напрямую от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки</typeparam>
        /// <param name="instance">Объект для которого проверяется наследование от типа <typeparamref name="TBase"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void InheritsFrom<TBase>(object instance, string paramName = "", string message = "")
        {
            InheritsFrom<TBase>(instance.GetType(), message.NotEmpty(Localization.Guard_InstanceNotInheritedOfType.Args(paramName, typeof(TBase))));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный тип
        /// не унаследован напрямую от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки.</typeparam>
        /// <param name="type">Тип <see cref="Type"/> для проверки унаследованости от типа <typeparamref name="TBase"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void InheritsFrom<TBase>(Type type, string message = "")
        {
            if (type.BaseType != typeof(TBase))
                throw new InvalidOperationException(message.NotEmpty(Localization.Guard_TypeNotInheritedOfType.Args(type, typeof(TBase))));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр не унаследован от типа <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Базовый тип для проверки</typeparam>
        /// <param name="instance">Объект для которого проверяется наследование от типа <typeparamref name="TBase"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsSubclassOf<TBase>(object instance, string paramName = "", string message = "")
        {
            if (!instance.GetType().IsSubclassOf(typeof(TBase)))
                throw new InvalidOperationException(message.NotEmpty(Localization.Guard_InstanceNotSubclassOfType.Args(paramName, typeof(TBase))));
        }

        #region [Implements]

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не реализует интерфейс <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">Интерфейс который должен реализовывать эеземпляр.</typeparam>
        /// <param name="instance">Экземпляр, для которого проверяется реазизация интерфейса <typeparamref name="TInterface"/>.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Implements<TInterface>(object instance, string paramName = "", string message = "")
        {
            Implements<TInterface>(instance.GetType(), message.NotEmpty(Localization.Guard_InstanceNotImplementInterface.Args(paramName, typeof(TInterface))));
        }

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный тип
        /// не реализует интерфейс <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса <paramref name="type"/> который должен быть реализован.</typeparam>
        /// <param name="type">Тип <see cref="Type"/>, для которого проверяется реализация интерфейса <typeparamref name="TInterface"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Implements<TInterface>(Type type, string message = "")
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(message.NotEmpty(Localization.Guard_TypeNotImplementInterface.Args(type, typeof(TInterface))));
        }

        #endregion

        /// <summary>
        /// Вызывает <see cref="InvalidOperationException"/> когда указанный экземпляр
        /// не указанного типа.
        /// </summary>
        /// <typeparam name="TType">Тип на соотвествие которому проверяется <paramref name="instance"/>.</typeparam>
        /// <param name="instance">Экземпляр, для которого выполняется проверка.</param>
        /// <param name="paramName">Наименование экемпляра</param>
        /// <param name="message">Сообщение для исключения <see cref="InvalidOperationException"/>.</param>
        public static void TypeOf<TType>(object instance, string paramName = "", string message = "")
        {
            if (!(instance is TType))
                throw new InvalidOperationException(message.NotEmpty(Localization.Guard_InstanceNotOfType.Args(paramName, typeof(TType))));
        }

        /// <summary>
        /// Вызывает <typeparamref name="TException"/>, когда экземпляр объекта не равен другому экземпляру объекта.
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="compare">Сравниваемый экземпляр объекта </param>
        /// <param name="instance">Экземпляр объекта, с которым сравнивается.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsEqual<TException>(object compare, object instance, string message) where TException : Exception
        {
            if (compare != instance)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <see cref="Exception"/>, когда экземпляр объекта не равен другому экземпляру объекта.
        /// </summary>
        /// <param name="compare">Сравниваемый экземпляр объекта </param>
        /// <param name="instance">Экземпляр объекта, с которым сравнивается.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsEqual(object compare, object instance, string message)
        {
            IsEqual<Exception>(compare, instance, message);
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentNullException"/>, когда экземпляр объекта является значением null.
        /// </summary>
        /// <param name="instance">Экземпляр объекта.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotNull(object instance, string paramName = "", string message = "")
        {
            if (instance is null)
                throw new ArgumentNullException(paramName, message.NotEmpty(Localization.Guard_ValueCantBeUndefined.Args(paramName)));
        }

        #region [IsNotEmpty]

        /// <summary>
        /// Вызывает <see cref="ArgumentException"/>, когда заданная строка является значением null, пустой строкой или строкой, состоящей только из пробельных символов.
        /// </summary>
        /// <param name="value">Заданная строка.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotEmpty(string value, string paramName = "", string message = "")
        {
            if (value.IsEmpty())
                throw new ArgumentException(paramName, message.NotEmpty(Localization.Guard_ValueCantBeEmpty.Args(paramName)));
        }

        /// <summary>
        /// Вызывает <see cref="ArgumentException"/>, когда заданный GUID является значением null или равен его значение содержит все 0
        /// </summary>
        /// <param name="value">Заданная строка.</param>
        /// <param name="paramName">Имя параметра, вызвавшего данное исключение.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void IsNotEmpty(Guid value, string paramName = "", string message = "")
        {
            if (value.IsEmpty())
                throw new ArgumentException(paramName, message.NotEmpty(Localization.Guard_ValueCantBeEmpty.Args(paramName)));
        }

        #endregion

        #region [Against]

        /// <summary>
        /// Вызывает <typeparamref name="TException"/> с указанным сообщением когда утверждение верно.
        /// </summary>
        /// <typeparam name="TException">Тип вызываемого исключения.</typeparam>
        /// <param name="assertion"> Условие для проверки. Если true, тогда вызывается исключение <typeparamref name="TException"/>.</param>
        /// <param name="message"> Строка сообщения для вызываемого исключения.</param>
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (assertion)
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        /// <summary>
        /// Вызывает <typeparamref name="TException"/> с указанным сообщением когда утверждение верно.
        /// </summary>
        /// <typeparam name="TException"> Тип вызываемого исключения.</typeparam>
        /// <param name="assertion">Условие для проверки. Если true, тогда вызывается исключение <typeparamref name="TException"/>.</param>
        /// <param name="message">Строка сообщения для вызываемого исключения.</param>
        public static void Against<TException>(Func<bool> assertion, string message) where TException : Exception
        {
            if (assertion())
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }

        #endregion
    }

}
