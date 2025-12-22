using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class NewRecipeViewModel : BaseViewModel
    {


        public NewRecipeViewModel()
        {
            // Устанавливаем режим по умолчанию
            Recipe.JointMode = JointMode.Torque;

            CreateRecipeCmd = new RelayCommand((arg) =>
            {
                try
                {
                    Recipe.Name = RecipeName;
                    RecipeCreated?.Invoke(this, Recipe);
                }
                catch(Exception ex)
                {
                    Error = ex.Message;
                    OnPropertyChanged(nameof(Error));
                }

            });

            CancelCmd = new RelayCommand((arg) =>
            {
                Canceled?.Invoke(this, Recipe);
            });

            SetModeCommand = new RelayCommand((mode) => SetMode((JointMode)mode));
        }

        void SetMode(JointMode newMode)
        {
            Recipe.JointMode = newMode;
            OnPropertyChanged(nameof(Recipe));
        }

        public event EventHandler<JointRecipeTable> RecipeCreated;

        public event EventHandler<JointRecipeTable> Canceled;
        public ICommand CreateRecipeCmd { get; set; }
        public ICommand CancelCmd { get; set; }
        public ICommand SetModeCommand { get; set; }
        public JointRecipeTable Recipe { get; set; } = new JointRecipeTable()
        {
            HEAD_OPEN_PULSES = 0.08f,
            TURNS_BREAK = 4.0f,
            Thread_step = 10,
            ThreadType = ThreadType.RIGHT,
            Box_Moni_Time = 10,
            Box_Len_Min = 240,
            Box_Len_Max = 270,
            Pre_Moni_Time = 20,
            Pre_Len_Min = 35,
            Pre_Len_Max = 95,
            MU_Moni_Time = 60,
            MU_Tq_Ref = 200,
            MU_Tq_Save = 2000,
            MU_TqSpeedRed_1 = 5000,
            MU_TqSpeedRed_2 = 10000,
            MU_Tq_Dump = 14000,
            MU_Tq_Max = 15000,
            MU_Tq_Min = 7000,
            MU_Tq_Opt = 15000,
            MU_TqShoulder_Min = 12000,
            MU_TqShoulder_Max = 12000,
            MU_Len_Speed_1 = 103,
            MU_Len_Speed_2 = 112,
            MU_Len_Dump = 114,
            MU_Len_Min = 108f,
            MU_Len_Max = 122.5f,
            MU_JVal_Speed_1 = 100,
            MU_JVal_Speed_2 = 100,
            MU_JVal_Dump = 100,
            MU_JVal_Min = 100,
            MU_JVal_Max = 100
        };


        public string RecipeName { get; set; }

        public string Error { get; set; }

    }
}
