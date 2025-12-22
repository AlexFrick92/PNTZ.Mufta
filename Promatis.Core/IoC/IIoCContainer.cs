using System;

namespace Promatis.Core
{
    /// <summary>
    /// Интерфейс, описывающий реализацию IoC контейнера 
    /// </summary>
    public interface IIoCContainer : IDisposable
    {
        /// <summary>
        /// Конфигурирует контейнер.
        /// </summary>
        void Configure();

        /// <summary>
        /// Получает из контейнера готовый экземпляр для указанного типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип</typeparam>
        /// <returns></returns>
        TService Resolve<TService>();
        /// <summary>
        /// Получает из контейнера готовый экземпляр для указанного типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип</typeparam>
        /// <returns></returns>
        TService Resolve<TService>(Type type);

        /// <summary>
        /// Возвращает экземпляр реализации для указанного типа сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <returns>Если сервис не зарегистрован, то значение по умолчанию</returns>
        TService TryResolve<TService>();

        #region Register

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        void Register(Type serviceType, Type implementationType);

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        /// <param name="key">Ключ</param>
        void Register(Type serviceType, Type implementationType, string key);

        /// <summary>
        /// Регистрирует в контейнере реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        void Register<TService, TImplementation>() where TImplementation : TService;

        /// <summary>
        /// Регистрирует в контейнере реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        /// <param name="key">Ключ</param>
        void Register<TService, TImplementation>(string key) where TImplementation : TService;

        #endregion

        #region RegisterDisposable

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        void RegisterDisposable(Type serviceType, Type implementationType);

        /// <summary>
        /// Регистрирует в контейнере реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        void RegisterDisposable<TService, TImplementation>() where TImplementation : TService;

        #endregion

        #region RegisterInstance

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="instance">Экземпляр</param>
        void RegisterInstance<TService>(TService instance);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="instance">Экземпляр реализации</param>
        void RegisterInstance(Type serviceType, object instance);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="instance">Экземпляр</param>
        /// <param name="key">Ключ</param>
        void RegisterInstance<TService>(TService instance, string key);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="instance">Экземпляр реализации</param>
        /// <param name="key">Ключ</param>
        void RegisterInstance(Type serviceType, object instance, string key);
        #endregion

        #region [Методы регистрации singleton типов]

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">ТИп реализации</param>
        void RegisterSingleton(Type serviceType, Type implementationType);

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;

        #endregion

        /// <summary>
        /// Проверяет зарегисрирован ли в контейнере указанный тип сервиса. <c>true</c> если зарегистрирован, иначе <c>false</c>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        bool IsRegistered<TService>();

        /// <summary>
        /// Удаляет все регистрации заданного типа сервиса в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        void UnregisterInstance<TService>();
    }
}
