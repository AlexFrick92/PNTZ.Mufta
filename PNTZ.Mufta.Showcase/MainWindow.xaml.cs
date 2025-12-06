using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PNTZ.Mufta.Showcase.Models;
using PNTZ.Mufta.Showcase.TestWindows;

namespace PNTZ.Mufta.Showcase
{
    /// <summary>
    /// Главное окно галереи контролов
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ControlInfo> _allControls;
        private string _searchText = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();

            // Загружаем контролы после полной инициализации UI
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls();
        }

        /// <summary>
        /// Инициализация списка доступных контролов
        /// </summary>
        private void InitializeControls()
        {
            _allControls = new List<ControlInfo>
            {
                new ControlInfo
                {
                    Name = "ChartView",
                    Category = "Графики",
                    Description = "График крутящего момента с поддержкой zoom, pan, константных линий и real-time обновлений. " +
                                  "Используется для визуализации процесса навёртки муфты.",
                    WindowType = typeof(ChartViewTestWindow)
                },

                new ControlInfo
                {
                    Name = "JointProcessChartView",
                    Category = "Графики",
                    Description = "Компонент для отображения графиков процесса навёртки муфты.",
                    WindowType = typeof(JointProcessChartViewTestWindow)
                },

                // Здесь будут добавляться другие контролы в будущем
                // Пример:
                // new ControlInfo
                // {
                //     Name = "StatusIndicator",
                //     Category = "Индикаторы",
                //     Description = "Индикатор статуса с цветовой индикацией состояния оборудования",
                //     WindowType = typeof(StatusIndicatorTestWindow)
                // },
            };
        }

        /// <summary>
        /// Загрузка списка контролов в UI
        /// </summary>
        private void LoadControls()
        {
            // Проверка на случай, если метод вызван до завершения InitializeComponent
            if (ControlsListBox == null || _allControls == null)
                return;

            var controlsToDisplay = string.IsNullOrWhiteSpace(_searchText)
                ? _allControls
                : FilterControls(_searchText);

            ControlsListBox.ItemsSource = controlsToDisplay;
            UpdateControlCount(controlsToDisplay.Count);
        }

        /// <summary>
        /// Фильтрация контролов по поисковому запросу
        /// </summary>
        private List<ControlInfo> FilterControls(string searchText)
        {
            searchText = searchText.ToLower();

            return _allControls.Where(c =>
                c.Name.ToLower().Contains(searchText) ||
                c.Description.ToLower().Contains(searchText) ||
                c.Category.ToLower().Contains(searchText)
            ).ToList();
        }

        /// <summary>
        /// Обновление счетчика контролов
        /// </summary>
        private void UpdateControlCount(int count)
        {
            ControlCountText.Text = count.ToString();
        }

        /// <summary>
        /// Обработчик изменения текста в поиске
        /// </summary>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Игнорируем placeholder текст
            if (textBox.Text == "Поиск контролов...")
            {
                _searchText = "";
            }
            else
            {
                _searchText = textBox.Text;
            }

            LoadControls();
        }

        /// <summary>
        /// Обработчик двойного клика по контролу - открывает окно тестирования
        /// </summary>
        private void ControlsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedControl();
        }

        /// <summary>
        /// Открытие окна тестирования для выбранного контрола
        /// </summary>
        private void OpenSelectedControl()
        {
            if (ControlsListBox.SelectedItem is ControlInfo controlInfo)
            {
                try
                {
                    // Создаем экземпляр окна тестирования
                    var testWindow = (Window)Activator.CreateInstance(controlInfo.WindowType);
                    testWindow.Owner = this;
                    testWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при открытии окна тестирования:\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    "Пожалуйста, выберите контрол из списка.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
