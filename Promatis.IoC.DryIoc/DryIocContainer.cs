using System;
using DryIoc;
using Promatis.Core;

namespace Promatis.IoC.DryIoc
{
    /// <summary>
    /// Реализация Ioc контейнера для DryIoc
    /// </summary>
    public class DryIocContainer : IIoCContainer
    {
        private readonly IContainer _container;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DryIocContainer"/>
        /// </summary>
        public DryIocContainer() => _container = new Container();

        public void Configure()
        {}

        /// <summary>
        /// Возвращает экземпляр реализации для указанного типа сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <returns></returns>
        public TService Resolve<TService>() => _container.Resolve<TService>();
        /// <summary>
        /// Возвращает экземпляр реализации для указанного типа сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public TService Resolve<TService>(Type type) => _container.Resolve<TService>(type);

        /// <summary>
        /// Возвращает экземпляр реализации для указанного типа сервиса, зарегистрированного в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <returns>Если сервис не зарегистрован, то значение по умолчанию</returns>
        public TService TryResolve<TService>() => _container.Resolve<TService>(IfUnresolved.ReturnDefault);

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        public void Register(Type serviceType, Type implementationType) => _container.Register(serviceType, implementationType);

        /// <summary>
        /// Регистрирует в контейнере тип реализации для типа сервиса
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public void Register<TService, TImplementation>() where TImplementation : TService => _container.Register<TService, TImplementation>();

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        /// <param name="key">Ключ</param>
        public void Register(Type serviceType, Type implementationType, string key) => _container.Register(serviceType, implementationType, serviceKey: key);

        /// <summary>
        /// Регистрирует в контейнере тип реализации для типа сервиса
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        /// <param name="key">Ключ</param>
        public void Register<TService, TImplementation>(string key) where TImplementation : TService => _container.Register<TService, TImplementation>(serviceKey: key);

        /// <summary>
        /// Регистрирует в контейнере реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">Тип реализации</param>
        public void RegisterDisposable(Type serviceType, Type implementationType) => _container.Register(serviceType, implementationType, setup: Setup.With(allowDisposableTransient: true));

        /// <summary>
        /// Регистрирует в контейнере тип реализации для типа сервиса
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public void RegisterDisposable<TService, TImplementation>() where TImplementation : TService => _container.Register<TService, TImplementation>(setup: Setup.With(allowDisposableTransient: true));

        #region RegisterInstance

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="instance">Экземпляр</param>
        public void RegisterInstance<TService>(TService instance) => _container.RegisterInstance(typeof(TService), instance);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="instance">Экземпляр реализации</param>
        public void RegisterInstance(Type serviceType, object instance) => _container.RegisterInstance(serviceType, instance);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="instance">Экземпляр</param>
        /// <param name="key">Ключ</param>
        public void RegisterInstance<TService>(TService instance, string key) => _container.RegisterInstance(typeof(TService), instance, serviceKey: key);

        /// <summary>
        /// Регистрирует в контейнере готовый экземпляр реализации для типа <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="instance">Экземпляр реализации</param>
        /// <param name="key">Ключ</param>
        public void RegisterInstance(Type serviceType, object instance, string key) => _container.RegisterInstance(serviceType, instance, serviceKey: key);

        #endregion

        #region [Методы регистрации singleton типов]

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <paramref name="implementationType"/> для <paramref name="serviceType"/>
        /// </summary>
        /// <param name="serviceType">Тип сервиса</param>
        /// <param name="implementationType">ТИп реализации</param>
        public void RegisterSingleton(Type serviceType, Type implementationType) =>
            _container.Register(serviceType, implementationType, Reuse.Singleton);

        /// <summary>
        /// Регистрирует в контейнере singleton реализацию <typeparamref name="TImplementation"/> для <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <typeparam name="TImplementation">Тип реализации</typeparam>
        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService =>
            _container.Register<TService, TImplementation>(Reuse.Singleton);

        #endregion

        /// <summary>
        /// Проверяет зарегисрирован ли в контейнере указанный тип сервиса. <c>true</c> если зарегистрирован, иначе <c>false</c>
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        public bool IsRegistered<TService>() => _container.IsRegistered<TService>();

        /// <summary>
        /// Удаляет все регистрации заданного типа сервиса в контейнере
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        public void UnregisterInstance<TService>() => _container.Unregister<TService>();

        #region IDisposable

        public void Dispose() => _container?.Dispose();

        #endregion
    }
}
