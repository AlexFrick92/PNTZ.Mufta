using Desktop.MVVM;
using DevExpress.Charts.Designer.Native;
using DevExpress.Xpo.DB;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View.Recipe;
using Promatis.Core.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using static PNTZ.Mufta.TPCApp.App;


namespace PNTZ.Mufta.TPCApp.ViewModel
{
    internal class RecipeViewModel : BaseViewModel
    {
        RecipeToPlc recipeLoader;
        ILogger logger;
        RepositoryContext repo;

        public RecipeViewModel(RecipeToPlc recipeLoader, ILogger logger, RepositoryContext repoContext)
        {
            this.logger = logger;
            this.recipeLoader = recipeLoader;
            repo = repoContext;
            


            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));

            JointRecipes = new ObservableCollection<JointRecipe>();
            foreach(JointRecipe jointRecipe in repo.LoadRecipes())
            {
                JointRecipes.Add(jointRecipe);
            }

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

            NewRecipeCommand = new RelayCommand((arg) =>
            {

                NewRecipeViewModel newRecvm = new NewRecipeViewModel();                
                NewRecipeView newRecipeView = new NewRecipeView(newRecvm);
                newRecvm.RecipeCreated += (o, r) =>
                {
                    EditRecipe = r;                    
                    OnPropertyChanged(nameof(EditRecipe));
                    newRecipeView.Close();
                    RecipeEditable = true;
                    OnPropertyChanged(nameof(RecipeEditable));
                };
                newRecvm.Canceled += (o, r) =>
                {
                    newRecipeView.Close();
                };


                newRecipeView.ShowDialog();
            });

            SaveRecipes = new RelayCommand((arg) =>
            {
                repo.SaveRecipes(JointRecipes);               
            });


            
            SaveRecipe = new RelayCommand((arg) =>
            {
                EditRecipe.TimeStamp = DateTime.UtcNow;

                if(!JointRecipes.Contains(EditRecipe))
                    JointRecipes.Add(EditRecipe);

            });
        }

        public ObservableCollection<JointRecipe> JointRecipes { get; set;  }

        JointRecipe selectedJointRecipe;
        public JointRecipe SelectedJointRecipe
        {
            get => selectedJointRecipe;
            set
            {
                selectedJointRecipe = value;
                EditRecipe = value;
                OnPropertyChanged(nameof(EditRecipe));
                RecipeEditable = true;
                OnPropertyChanged(nameof(RecipeEditable));
            }
        }
        public bool RecipeEditable { get; set; } = false;
        public JointRecipe EditRecipe { get; set; } = new JointRecipe();

        public ICommand SetModeCommand { get; set; }

        public ICommand LoadRecipeCommand { get; set; }

        public ICommand NewRecipeCommand { get; set; }

        public ICommand SaveRecipes { get; set; }

        public ICommand SaveRecipe { get; set; }

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
