using System;
using System.Windows;
using System.Windows.Threading;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;
using PNTZ.Mufta.Showcase.Helper;
using PNTZ.Mufta.Showcase.Data;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования контрола JointView
    /// </summary>
    public partial class JointViewTestWindow : Window
    {
        private JointViewModel _viewModel;
        private MockRecipeLoader _mockRecipeLoader;
        private MockJointProcessWorker _mockJointProcessWorker;

        public JointViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeView();
        }

        private void InitializeView()
        {
            UpdateIntervalTextBox.Text = "10";
        }

        private void InitializeViewModel()
        {
            _mockRecipeLoader = new MockRecipeLoader();
            _mockJointProcessWorker = new MockJointProcessWorker();
            _mockRecipeLoader.RecipeLoaded += OnMockRecipeLoaded;
            _mockRecipeLoader.RecipeLoaded += (s, r) => _mockJointProcessWorker.SetActualRecipe(r);
            _viewModel = new JointViewModel(_mockJointProcessWorker, _mockRecipeLoader, new ConsoleLogger());
            JointView.DataContext = _viewModel;
            
            _mockJointProcessWorker.Initialize();
        }

        /// <summary>
        /// Обработчик успешной загрузки рецепта из мока (для обновления статуса)
        /// </summary>
        private void OnMockRecipeLoaded(object sender, JointRecipe recipe)
        {
            // Обновляем статус в UI (нужно вызвать в UI потоке)
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"Рецепт загружен: {recipe.Name} (Режим: {recipe.JointMode})");
            });
        }

        public int UpdateInterval { get; set; }


        private void UpdateInterval_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                UpdateInterval = int.Parse(UpdateIntervalTextBox.Text);
            }
            catch
            {
                // Игнорируем ошибки парсинга
            }
        }

        /// <summary>
        /// Обновляет статусную строку
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusText.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        private void BtnStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем интервал обновления в MockJointProcessWorker
            _mockJointProcessWorker.UpdateIntervalMs = UpdateInterval;

            // Запускаем симуляцию
            _mockJointProcessWorker.Start();

            UpdateStatus($"Симуляция запущена с интервалом {UpdateInterval} мс");
        }

        private void BtnStopSimulation_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем симуляцию
            _mockJointProcessWorker.Stop();

            UpdateStatus("Симуляция остановлена");
        }

        private void BtnLoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Length...");
            _mockRecipeLoader.LoadRecipeLength();
        }

        private void BtnLoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque...");
            _mockRecipeLoader.LoadRecipeTorque();
        }

        private void BtnLoadRecipeTorqueLength_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque+Length...");
            _mockRecipeLoader.LoadRecipeTorqueLength();
        }

        private void BtnLoadRecipeTorqueShoulder_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Загрузка рецепта Torque+Shoulder...");
            _mockRecipeLoader.LoadRecipeTorqueShoulder();
        }
    }
}
