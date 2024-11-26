using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.View.Recipe;
using Promatis.Core.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class RecipeViewModel : BaseViewModel
    {
        RecipeToPlc recipeLoader;
        ILogger logger;

        public RecipeViewModel(RecipeToPlc recipeLoader, ILogger logger)
        {
            this.logger = logger;
            this.recipeLoader = recipeLoader;
            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));
            LoadRecipeCommand = new RelayCommand((arg) =>
            Task.Run(async () =>
            {
                try
                {
                    await recipeLoader.LoadRecipeAsync(EditRecipe);
                }
                catch (Exception ex)
                {
                    logger.Info(ex.Message);
                }

            }));
        }

        public JointRecipe EditRecipe { get; set; } = new JointRecipe();

        public ICommand SetModeCommand { get; set; }

        public ICommand LoadRecipeCommand { get; set; }

        public bool IsVisible { get; set; } = false;

        void SetMode(JointMode newMode)
        {
            IsVisible = true;
            OnPropertyChanged(nameof(IsVisible));

            EditRecipe.JointMode = newMode;
            OnPropertyChanged(nameof(EditRecipe));

            AppInstance.Logger.Info("Установлен новый режим:" + newMode.ToString());
        }
    }
}
