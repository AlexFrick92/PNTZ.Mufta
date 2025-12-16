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
    /// Attached property helper для отслеживания ошибок валидации во вложенных ParameterDisplayControl
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
            // Находим все ParameterDisplayControl в визуальном дереве
            var controls = FindVisualChildren<ParameterDisplayControl>(root).ToList();

            if (!_subscriptions.ContainsKey(root))
            {
                _subscriptions[root] = new List<DependencyPropertyDescriptor>();
            }

            // Подписываемся на изменения IsValidationError для каждого контрола
            foreach (var control in controls)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(
                    ParameterDisplayControl.IsValidationErrorProperty,
                    typeof(ParameterDisplayControl));

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
                var controls = FindVisualChildren<ParameterDisplayControl>(root).ToList();

                foreach (var control in controls)
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
            var controls = FindVisualChildren<ParameterDisplayControl>(root);
            bool hasErrors = controls.Any(c => c.IsValidationError);

            SetHasValidationErrors(root, hasErrors);
        }

        /// <summary>
        /// Рекурсивный поиск всех дочерних элементов заданного типа в визуальном дереве
        /// </summary>
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                yield break;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                // Рекурсивно проверяем дочерние элементы
                foreach (T descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        #endregion
    }
}
