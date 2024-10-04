using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;


using Desktop.MVVM;

using static PNTZ.Mufta.App.App;    
using PNTZ.Mufta.App.Domain.Joint;
using System.Collections.ObjectModel;


namespace PNTZ.Mufta.App.ViewModel
{
    public class CreateRecipeViewModel : BaseViewModel
    {
        public CreateRecipeViewModel()
        {
            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));

            SaveRecipeCommand = new RelayCommand((arg) => SaveRecipe(JointRecipe));

            LoadRecipeCommand = new RelayCommand(async (arg) =>
            {


                Task task = AppInstance.CamRecipeLoader.LoadRecipeAsync(JointRecipe);

                try
                {
                    await task;
                    AppInstance.LoadedRecipe = JointRecipe;
                }
                catch (Exception ex)
                {
                    AppInstance.AppCli.WriteLine(ex.Message);
                }
            });

            SavedRecipes = new ObservableCollection<JointRecipe>
            { 
                new JointRecipe() { Name = "Первый рецепт"},
                new JointRecipe() { Name = "Второй рецепт"},
            };

            OnPropertyChanged(nameof(SavedRecipes));

        }
        

        public ObservableCollection<JointRecipe> SavedRecipes { get; private set; } 
        public JointRecipe SelectedSavedRecipe { get; set; }

        public ICommand SetModeCommand { get; set; }

        void SetMode(JointMode newMode)
        {
            SelectedMode = newMode;

            Console.WriteLine("Установлен новый режим:" + newMode.ToString());
        }

        public JointRecipe JointRecipe { get; set; } = new JointRecipe();

        public ICommand SaveRecipeCommand { get; set; }
        public ICommand LoadRecipeCommand { get; set; }
        void SaveRecipe(JointRecipe newRecipe)
        {
            try
            {
                AppInstance.SaveJointRecipe(newRecipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        JointMode _selectedMode;
        public JointMode SelectedMode
        {
            get => _selectedMode;
            set
            {
                if (_selectedMode != value)
                {
                    _selectedMode = value;
                    JointRecipe.MU_Makeup_Mode = AppInstance.JointModeToMakeUpMode(value);
                    OnPropertyChanged(nameof(SelectedMode));                    
                }
            }
        }



    }
}
