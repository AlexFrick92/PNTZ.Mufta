using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Promatis.Core
{
    /// <summary>
    /// Общая фабрика для получения объекта через Reflection
    /// </summary>
    public abstract class FactoryBase<T> where T : class
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<int, ConstructorInfo> Constructors = new ConcurrentDictionary<int, ConstructorInfo>();

        /// <summary>
        /// <para>Создает объект путем получения конструктора через Reflection </para>
        /// <para>Конструктор объекта вытаскивается только при первом обращении и сохраняется.
        /// При последующих обращениях создаем объект через готовый конструктор</para>
        /// </summary>
        /// <returns></returns>
        protected virtual T Create(params object[] args)
        {
            var types = args.Select(p => p?.GetType() ?? typeof(object)).ToArray();
            int hash = HashCode.Of(types);

            if (!Constructors.TryGetValue(hash, out var constructor))
            {
                constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);

                if (constructor == null && args.Any(x => x == null))
                {
                    // если есть нулевые значения - подбираем подходящий конструктор 
                    var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(
                            x =>
                            {
                                var parameters = x.GetParameters();
                                if (parameters.Length != args.Length)
                                    return false;
                                var isCompatible = true;
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    if (args[i] == null && parameters[i].GetType().IsValueType)
                                    {
                                        isCompatible = false;
                                        break;
                                    }
                                }
                                return isCompatible;
                            }).ToList();

                    Guard.Against<ArgumentException>(constructors.Count > 1,
                        $"Type {typeof(T)} contains same constructor with those parameters {string.Join(", ", types.Select(x => x.Name))}");
                    constructor = constructors.FirstOrDefault();

                }
                Guard.Against<ArgumentException>(constructor == null,
                    $"For type {typeof(T)} not defined {(args.Any() ? "constructor with  parameters " + string.Join(", ", types.Select(x => x.Name)) : "parameterless constructor")}");
                Constructors.TryAdd(hash, constructor);
            }
            try
            {
                return (T)constructor?.Invoke(args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// Создает представление объекта через беспараметризованный контруктор объекта
        /// </summary>
        /// <typeparam name="TView">Тип представления</typeparam>
        /// <param name="converter">Конвертер</param>
        /// <returns>Сформированное представление</returns>
        protected TView CreateView<TView>(IConverter<T, TView> converter)
        {
            var entity = Create();
            return converter.Convert(entity);
        }

    }
}
