using System;

namespace Promatis.Core
{
    /// <summary>
    /// Реализация Dependency Injection
    /// </summary>
    public static class IoC
    {
        #region [Методы разрешения зависимостей]

        /// <summary>
        /// Возвращает экземпляр реализации для указанного типа сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>() => IocContext.Container.Resolve<TService>();
        /// <summary>
        /// Возвращает экземпляр реализации для указанного интерфейса сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="IService">Тип сервиса</typeparam>
        /// <returns></returns>
        public static IService Resolve<IService>(Type type) => IocContext.Container.Resolve<IService>(type);

        /// <summary>
        /// Пытается получить экземпляра реализации для указанного типа сервиса, зарегистрированного в контейнере.
        /// Если тип не зарегистрирован, то возвращает <c>false</c>, иначе <c>true</c>. Экземпляр реализации сервиса возвращается через out параметр <paramref name="service"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <returns>Если сервис не зарегистрован, то значение по умолчанию</returns>
        public static bool TryResolve<TService>(out TService service)
        {
            service = IocContext.Container.TryResolve<TService>();
            return service != null;
        }

        #endregion

        #region [Методы регистрации произвольных типов]

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        public static void Register(Type serviceType, Type implementationType) => IocContext.Container.Register(serviceType, implementationType);

        /// <summary>
        /// Регистрирует в контейнере тип реализации для типа сервиса
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public static void Register<TService, TImplementation>() where TImplementation : TService => IocContext.Container.Register<TService, TImplementation>();

        #endregion

        #region [Методы регистрации Disposable типов]


        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        public static void RegisterDisposable(Type serviceType, Type implementationType)
        {
            IocContext.Container.RegisterDisposable(serviceType, implementationType);
        }

        /// <summary>
        /// Регистрирует в контейнере тип реализации для типа сервиса
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public static void RegisterDisposable<TService, TImplementation>() where TImplementation : TService
        {
            IocContext.Container.RegisterDisposable<TService, TImplementation>();
        }

        #endregion

        #region [Методы регистрации готовоых экземпляров]

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="instance">Экземпляр</param>
        public static void RegisterInstance<TService>(TService instance) => IocContext.Container.RegisterInstance(typeof(TService), instance);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="instance">Экземпляр реализации</param>
        public static void RegisterInstance(Type serviceType, object instance) => IocContext.Container.RegisterInstance(serviceType, instance);

        #endregion

        #region [Методы регистрации singleton типов]

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">ТИп реализации</param>
        public static void RegisterSingleton(Type serviceType, Type implementationType) =>
            IocContext.Container.RegisterSingleton(serviceType, implementationType);

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public static void RegisterSingleton<TService, TImplementation>() where TImplementation : TService =>
            IocContext.Container.RegisterSingleton<TService, TImplementation>();

        #endregion

        /// <summary>
        /// Проверяет зарегисрирован ли в контейнере указанный тип сервиса. <c>true</c> если зарегистрирован, иначе <c>false</c>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        public static bool IsRegistered<TService>() => IocContext.Container.IsRegistered<TService>();

        /// <summary>
        /// Удаляет все регистрации заданного типа сервиса в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        public static void UnregisterInstance<TService>() => IocContext.Container.UnregisterInstance<TService>();

        /// <summary>
        /// Удаляет все зарегистрированные типы сервисов из контейнера
        /// </summary>
        public static void ClearContainer() => IocContext.ClearContainer();
    }
}
