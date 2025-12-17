using PNTZ.Mufta.TPCApp.ViewModel.Recipe;
using System.Windows;

namespace PNTZ.Mufta.TPCApp.View.Recipe
{
    /// <summary>
    /// Interaction logic for LoadingRecipeView.xaml
    /// </summary>
    public partial class LoadingRecipeView : Window
    {
        public LoadingRecipeView(LoadingRecipeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
