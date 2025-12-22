using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Recipe
{
    /// <summary>
    /// Interaction logic for LoadingRecipeView.xaml
    /// </summary>
    public partial class LoadingRecipeView : Window
    {
        private readonly LoadingRecipeViewModel _viewModel;

        public LoadingRecipeView(LoadingRecipeViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;

            // Подписываемся на события
            _viewModel.CloseRequested += (s, e) => Close();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Запускаем загрузку после того, как окно отобразилось
            await _viewModel.StartLoadingAsync();
        }
    }
}
