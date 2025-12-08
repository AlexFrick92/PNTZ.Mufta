using System;
using System.Windows;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;

namespace PNTZ.Mufta.Showcase.TestWindows
{
    /// <summary>
    /// Окно для тестирования JointProcessChartView
    /// </summary>
    public partial class JointProcessChartViewTestWindow : Window
    {
        private JointProcessChartViewModel _viewModel;

        public JointProcessChartViewTestWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        /// <summary>
        /// Инициализация ViewModel для JointProcessChartView
        /// </summary>
        private void InitializeViewModel()
        {
            _viewModel = new JointProcessChartViewModel();
            JointChartView.DataContext = _viewModel;
        }

        /// <summary>
        /// Загрузить тестовый рецепт по длине
        /// </summary>
        private void LoadRecipeLength_Click(object sender, RoutedEventArgs e)
        {
            var recipe = CreateTestRecipeLength();
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Загрузить тестовый рецепт по моменту
        /// </summary>
        private void LoadRecipeTorque_Click(object sender, RoutedEventArgs e)
        {
            var recipe = CreateTestRecipeTorque();
            _viewModel.UpdateRecipe(recipe);
            StatusText.Text = $"Загружен рецепт: {recipe.Name} (режим: {recipe.JointMode})";
        }

        /// <summary>
        /// Создать тестовый рецепт по длине
        /// </summary>
        private JointRecipe CreateTestRecipeLength()
        {
            return new JointRecipe
            {
                Id = Guid.NewGuid(),
                Name = "TEST_LENGTH",
                JointMode = JointMode.Length,
                SelectedThreadType = ThreadType.RIGHT,

                // Общие данные
                HEAD_OPEN_PULSES = 100f,
                TURNS_BREAK = 0.5f,
                PLC_PROG_NR = 1,
                LOG_NO = 1,
                Tq_UNIT = 1,
                Thread_step = 25.4f,
                PIPE_TYPE = "TEST_PIPE",

                // Параметры муфты
                Box_Moni_Time = 5000,
                Box_Len_Min = 10f,
                Box_Len_Max = 50f,

                // Параметры преднавёртки
                Pre_Moni_Time = 10000,
                Pre_Len_Max = 100f,
                Pre_Len_Min = 20f,

                // Параметры силового свинчивания общие
                MU_Moni_Time = 15000,
                MU_Tq_Ref = 5000f,
                MU_Tq_Save = 4500f,
                MU_TqSpeedRed_1 = 3000f,
                MU_TqSpeedRed_2 = 4000f,
                MU_Tq_Dump = 2000f,
                MU_Tq_Max = 8000f,
                MU_Tq_Min = 3000f,
                MU_Tq_Opt = 6000f,
                MU_TqShoulder_Min = 2500f,
                MU_TqShoulder_Max = 3500f,

                // Параметры силового свинчивания по длине
                MU_Len_Speed_1 = 50f,
                MU_Len_Speed_2 = 30f,
                MU_Len_Dump = 150f,
                MU_Len_Min = 200f,
                MU_Len_Max = 300f,

                // Параметры силового свинчивания по J
                MU_JVal_Speed_1 = 100f,
                MU_JVal_Speed_2 = 80f,
                MU_JVal_Dump = 500f,
                MU_JVal_Min = 1000f,
                MU_JVal_Max = 2000f,

                TimeStamp = DateTime.Now
            };
        }

        /// <summary>
        /// Создать тестовый рецепт по моменту
        /// </summary>
        private JointRecipe CreateTestRecipeTorque()
        {
            return new JointRecipe
            {
                Id = Guid.NewGuid(),
                Name = "TEST_TORQUE",
                JointMode = JointMode.Torque,
                SelectedThreadType = ThreadType.RIGHT,

                // Общие данные
                HEAD_OPEN_PULSES = 120f,
                TURNS_BREAK = 0.3f,
                PLC_PROG_NR = 2,
                LOG_NO = 2,
                Tq_UNIT = 1,
                Thread_step = 25.4f,
                PIPE_TYPE = "TEST_PIPE_TQ",

                // Параметры муфты
                Box_Moni_Time = 6000,
                Box_Len_Min = 15f,
                Box_Len_Max = 60f,

                // Параметры преднавёртки
                Pre_Moni_Time = 12000,
                Pre_Len_Max = 120f,
                Pre_Len_Min = 25f,

                // Параметры силового свинчивания общие
                MU_Moni_Time = 18000,
                MU_Tq_Ref = 7000f,
                MU_Tq_Save = 6500f,
                MU_TqSpeedRed_1 = 4000f,
                MU_TqSpeedRed_2 = 5500f,
                MU_Tq_Dump = 3000f,
                MU_Tq_Max = 10000f,
                MU_Tq_Min = 4000f,
                MU_Tq_Opt = 8000f,
                MU_TqShoulder_Min = 3000f,
                MU_TqShoulder_Max = 4000f,

                // Параметры силового свинчивания по длине
                MU_Len_Speed_1 = 60f,
                MU_Len_Speed_2 = 40f,
                MU_Len_Dump = 180f,
                MU_Len_Min = 250f,
                MU_Len_Max = 350f,

                // Параметры силового свинчивания по J
                MU_JVal_Speed_1 = 120f,
                MU_JVal_Speed_2 = 90f,
                MU_JVal_Dump = 600f,
                MU_JVal_Min = 1200f,
                MU_JVal_Max = 2500f,

                TimeStamp = DateTime.Now
            };
        }
    }
}
