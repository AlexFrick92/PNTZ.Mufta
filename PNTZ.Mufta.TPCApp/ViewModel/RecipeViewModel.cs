using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.View.Recipe;
using System.Windows.Input;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class RecipeViewModel : BaseViewModel
    {

        public RecipeViewModel()
        {
            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));
        }

        public JointRecipe EditRecipe { get; set; } = new JointRecipe();

        public ICommand SetModeCommand { get; set; }

        void SetMode(JointMode newMode)
        {
            EditRecipe.JointMode = newMode;
            OnPropertyChanged(nameof(EditRecipe));

            AppInstance.Logger.Info("Установлен новый режим:" + newMode.ToString());
        }              
    }
}
