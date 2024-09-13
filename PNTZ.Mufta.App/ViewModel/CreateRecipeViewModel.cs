using System;
using System.Windows.Input;

using Desktop.MVVM;

using PNTZ.Mufta.App.Domain.Joint;

using static PNTZ.Mufta.App.Global.Methods;

namespace PNTZ.Mufta.App.ViewModel
{
    public class CreateRecipeViewModel : BaseViewModel
    {
        public CreateRecipeViewModel()
        {
            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));

            SaveRecipeCommand = new RelayCommand((arg) => SaveRecipe(JointRecipe));
        }
        public int SomeParamValue { get; set; } = 5;

        public ICommand SetModeCommand { get; set; }

        void SetMode(JointMode newMode)
        {
            SelectedMode = newMode;

            Console.WriteLine("Установлен новый режим:" + newMode.ToString());
        }

        public JointRecipe JointRecipe { get; set; } = new JointRecipe();

        public ICommand SaveRecipeCommand { get; set; }
        void SaveRecipe(JointRecipe newRecipe)
        {
            try
            {
                SaveJointRecipe(newRecipe);
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
                    JointRecipe.MU_Makeup_Mode = JointModeToMakeUpMode(value);
                    OnPropertyChanged(nameof(SelectedMode));                    
                }
            }
        }



    }
}
