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


            UpdateSavedRecipeList();                               

        }
        

        public ObservableCollection<JointRecipe> SavedRecipes { get; private set; }


        JointRecipe _selectedSavedRecipe;
        public JointRecipe SelectedSavedRecipe 
        { 
            get
            {
                return _selectedSavedRecipe;
            }
            set
            {
                if (value != null)
                {
                    SelectedMode = value.JointMode;
                }
                this.JointRecipe = value;
                OnPropertyChanged(nameof(JointRecipe));
                _selectedSavedRecipe = value;
            }
        }

        public ICommand SetModeCommand { get; set; }

        void SetMode(JointMode newMode)
        {
            SelectedMode = newMode;

            Console.WriteLine("Установлен новый режим:" + newMode.ToString());
        }

        public JointRecipe JointRecipe { get; set; } = new JointRecipe();

        public ICommand SaveRecipeCommand { get; set; }
        public ICommand LoadRecipeCommand { get; set; }

        void UpdateSavedRecipeList()
        {
            SavedRecipes = new ObservableCollection<JointRecipe>(AppInstance.OpenJointRecipesFolder());
            OnPropertyChanged(nameof(SavedRecipes));
        }

        void SaveRecipe(JointRecipe newRecipe)
        {
            try
            {
                AppInstance.SaveJointRecipe(newRecipe);
                UpdateSavedRecipeList();

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
                    JointRecipe.JointMode = value;
                    JointRecipe.MU_Makeup_Mode = AppInstance.JointModeToMakeUpMode(value);
                    OnPropertyChanged(nameof(SelectedMode));                    
                }
            }
        }



    }
}
