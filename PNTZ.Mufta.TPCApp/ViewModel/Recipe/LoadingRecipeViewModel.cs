using Desktop.MVVM;

namespace PNTZ.Mufta.TPCApp.ViewModel.Recipe
{
    public class LoadingRecipeViewModel : BaseViewModel
    {
        private string _recipeName;

        public LoadingRecipeViewModel(string recipeName)
        {
            _recipeName = recipeName;
        }

        /// <summary>
        /// Название загружаемого рецепта
        /// </summary>
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                _recipeName = value;
                OnPropertyChanged(nameof(RecipeName));
            }
        }
    }
}
