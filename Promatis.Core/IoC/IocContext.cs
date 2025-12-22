using Promatis.Core.Resources;
using System;

namespace Promatis.Core
{
    /// <summary>
    /// Контекст контейнера Inversion Of Control
    /// </summary>
    public class IocContext
    {
        private static volatile IIoCContainer _container;
        private static readonly object Sync = new object();

        private IocContext()
        {
        }

        /// <summary>
        /// Инициализирует контекст инверсии управления
        /// </summary>
        /// <param name="container"></param>
        public static void Initialize(IIoCContainer container)
        {
            Guard.IsNotNull(container, nameof(container));

            if (_container == null)
            {
                lock (Sync)
                    if (_container == null)
                    {
                        _container = container;
                        _container.Configure();
                    }
            }
            else
                throw new Exception(Localization.IocContext_ContextAlreadyInitialized);
        }

        /// <summary>
        /// Экземпляр IoC контейнера
        /// </summary>
        public static IIoCContainer Container
        { 
            get
            {
                Guard.Against<Exception>(_container == null, Localization.IocContext_ContextIsNotInitialized);
                return _container;
            }
        }

		/// <summary>
		/// Удаляет текущий экземпляр IoC контейнера из контекста с вызовом Dispose у экземпляра контейнера
		/// </summary>
	    public static void ClearContainer()
	    {
			if (_container != null)
			{
				lock (Sync)
					if (_container != null)
					{
						_container.Dispose();
						_container = null;
					}
			}
	    }
    }
}
