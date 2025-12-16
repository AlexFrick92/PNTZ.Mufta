using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PNTZ.Mufta.TPCApp.View.Control;

namespace PNTZ.Mufta.TPCApp.View.Helpers
{
    /// <summary>
    /// Attached property helper для отслеживания ошибок валидации во вложенных контролах, реализующих IValidatable
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly Dictionary<DependencyObject, List<DependencyPropertyDescriptor>> _subscriptions
            = new Dictionary<DependencyObject, List<DependencyPropertyDescriptor>>();

        #region IsEnabled Attached Property

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(ValidationHelper),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    // Подписываемся на событие Loaded, чтобы дождаться полной загрузки визуального дерева
                    element.Loaded += OnElementLoaded;
                }
                else
                {
                    // Отписываемся от всех событий
                    element.Loaded -= OnElementLoaded;
                    UnsubscribeFromValidationChanges(element);
                }
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Отписываемся от события Loaded
                element.Loaded -= OnElementLoaded;

                // Начинаем мониторинг валидации
                SubscribeToValidationChanges(element);

                // Первоначальная проверка
                UpdateValidationState(element);
            }
        }

        #endregion

        #region HasValidationErrors Attached Property

        public static readonly DependencyProperty HasValidationErrorsProperty =
            DependencyProperty.RegisterAttached(
                "HasValidationErrors",
                typeof(bool),
                typeof(ValidationHelper),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static bool GetHasValidationErrors(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasValidationErrorsProperty);
        }

        public static void SetHasValidationErrors(DependencyObject obj, bool value)
        {
            obj.SetValue(HasValidationErrorsProperty, value);
        }

        #endregion

        #region Private Methods

        private static void SubscribeToValidationChanges(DependencyObject root)
        {
            // Находим все контролы, реализующие IValidatable в визуальном дереве
            var validatableControls = FindValidatableControls(root).ToList();

            if (!_subscriptions.ContainsKey(root))
            {
                _subscriptions[root] = new List<DependencyPropertyDescriptor>();
            }

            // Подписываемся на изменения IsValidationError для каждого контрола
            foreach (var control in validatableControls)
            {
                // Ищем DependencyProperty IsValidationError через имя
                var descriptor = DependencyPropertyDescriptor.FromName(
                    "IsValidationError",
                    control.GetType(),
                    control.GetType());

                if (descriptor != null)
                {
                    descriptor.AddValueChanged(control, (s, e) => OnValidationErrorChanged(root));
                    _subscriptions[root].Add(descriptor);
                }
            }
        }

        private static void UnsubscribeFromValidationChanges(DependencyObject root)
        {
            if (_subscriptions.ContainsKey(root))
            {
                var validatableControls = FindValidatableControls(root).ToList();

                foreach (var control in validatableControls)
                {
                    foreach (var descriptor in _subscriptions[root])
                    {
                        descriptor.RemoveValueChanged(control, (s, e) => OnValidationErrorChanged(root));
                    }
                }

                _subscriptions.Remove(root);
            }
        }

        private static void OnValidationErrorChanged(DependencyObject root)
        {
            UpdateValidationState(root);
        }

        private static void UpdateValidationState(DependencyObject root)
        {
            // Проверяем, есть ли хотя бы один контрол с ошибкой валидации
            var validatableControls = FindValidatableControls(root);
            bool hasErrors = validatableControls.Any(c => c.IsValidationError);

            SetHasValidationErrors(root, hasErrors);
        }

        /// <summary>
        /// Рекурсивный поиск всех контролов, реализующих IValidatable, в визуальном дереве
        /// </summary>
        private static IEnumerable<IValidatable> FindValidatableControls(DependencyObject parent)
        {
            if (parent == null)
                yield break;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                // Проверяем, реализует ли контрол IValidatable
                if (child is IValidatable validatable)
                {
                    yield return validatable;
                }

                // Рекурсивно проверяем дочерние элементы
                foreach (IValidatable descendant in FindValidatableControls(child))
                {
                    yield return descendant;
                }
            }
        }

        #endregion
    }
}
