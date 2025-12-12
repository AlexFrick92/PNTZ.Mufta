using System;
using System.Windows.Threading;

using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.View.Control;


namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// ViewModel для отображения данных процесса свинчивания
    /// </summary>
    public class JointProcessDataViewModel : BaseViewModel
    {
        // Интервал обновления отображаемых данных в миллисекундах
        private const int ACTUAL_POINT_UPDATE_INTERVAL = 50;

        private JointRecipe _loadedRecipe;
        private JointResult _jointResult;
        //Время в секундах с начала процесса стыковки
        private int _secondsFromBeginJointing;
        //Статус оценки результата
        private ParameterState _resultTotalState;
        private ParameterState _resultTorqueState;
        private ParameterState _resultLengthState;
        private ParameterState _resultShoulderState;
        //Таймер времени свинчивания
        private DispatcherTimer _timer;
        private DateTime _jointingStartTime;
        //Таймер обновления ActualPoint (throttling)
        private DispatcherTimer _actualPointUpdateTimer;

        #region MVVM свойства для UI       
        /// <summary>
        /// Загруженный рецепт
        /// </summary>
        public JointRecipe LoadedRecipe
        {
            get { return _loadedRecipe; }
            set 
            { 
                _loadedRecipe = value; 
                OnPropertyChanged(nameof(LoadedRecipe)); 
                OnPropertyChanged(nameof(IsLengthMode));
                OnPropertyChanged(nameof(IsShoulderMode));
            }
        }
        /// <summary>
        /// Результат стыковки
        /// </summary>
        public JointResult JointResult
        {
            get { return _jointResult; }
            set { _jointResult = value; OnPropertyChanged(nameof(JointResult)); }
        }
        /// <summary>
        /// Количество секунд с начала стыковки
        /// </summary>
        public int SecondsFromBeginJointing
        {
            get { return _secondsFromBeginJointing; }
            set { _secondsFromBeginJointing = value; OnPropertyChanged(nameof(SecondsFromBeginJointing)); }
        }
        /// <summary>
        /// Статус общей оценки результата
        /// </summary>
        public ParameterState ResultTotalState
        {
            get { return _resultTotalState; }
            set { _resultTotalState = value; OnPropertyChanged(nameof(ResultTotalState)); }
        }
        /// <summary>
        /// Статус момент - если превышен/не достигнут порог
        /// </summary>
        public ParameterState ResultTorqueState
        {
            get { return _resultTorqueState; }
            set { _resultTorqueState = value; OnPropertyChanged(nameof(ResultTorqueState)); }
        }
        /// <summary>
        /// Статус длины - если превышен/не достигнут порог
        /// </summary>
        public ParameterState ResultLengthState
        {
            get { return _resultLengthState; }
            set { _resultLengthState = value; OnPropertyChanged(nameof(ResultLengthState)); }
        }
        /// <summary>
        /// Статус плеча - если превышен/не достигнут порог
        /// </summary>
        public ParameterState ResultShoulderState
        {
            get { return _resultShoulderState; }
            set { _resultShoulderState = value; OnPropertyChanged(nameof(ResultShoulderState)); }
        }
        /// <summary>
        /// Выбранный режим "по длине" - для отображения соответствующих параметров
        /// </summary>
        public bool IsLengthMode => LoadedRecipe != null && (LoadedRecipe.JointMode == JointMode.Length || LoadedRecipe.JointMode == JointMode.TorqueLength);
        /// <summary>
        /// Выбранный режим "по плечу" - для отображения соответствующих параметров
        /// </summary>
        public bool IsShoulderMode => LoadedRecipe != null && LoadedRecipe.JointMode == JointMode.TorqueShoulder;

        #endregion

        public JointProcessDataViewModel()
        {
            InitializeActualPointTimer();
            _actualPointUpdateTimer.Start();
        }

        #region Публичные свойтва и методы

        /// <summary>
        /// Показания датчиков
        /// </summary>
        public TqTnLenPoint ActualPoint { get; set; }        
        /// <summary>
        /// Задать новый рецепт
        /// </summary>
        /// <param name="jointRecipe"></param>
        public void UpdateRecipe(JointRecipe jointRecipe)
        {
            LoadedRecipe = jointRecipe;
        }
        public void PipeAppear()
        {
            ResetResultsState();
        }
        /// <summary>
        /// Начать новое свинчивание
        /// </summary>
        public void BeginNewJointing()
        {
            ResetResultsState();
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateTimer_Tick;
            _jointingStartTime = DateTime.Now;
            SecondsFromBeginJointing = 0;
            _timer.Start();
        }
        public void FinishJointing(JointResult jointResult)
        {
            _timer.Stop();
            _timer.Tick -= UpdateTimer_Tick;
            JointResult = jointResult;

            // Установка статуса на основе ResultTotal
            if (jointResult != null)
            {
                switch (jointResult.ResultTotal)
                {
                    case 1:
                        ResultTotalState = ParameterState.Good;
                        break;
                    case 2:
                        ResultTotalState = ParameterState.Bad;
                        break;
                    default:
                        ResultTotalState = ParameterState.Normal;
                        break;
                }

                if(jointResult.EvaluationVerdict != null)
                {
                    ResultTorqueState = jointResult.EvaluationVerdict.TorqueOk ? ParameterState.Good : ParameterState.Bad;
                    ResultLengthState = jointResult.EvaluationVerdict.LentghOk ? ParameterState.Good : ParameterState.Bad;
                    ResultShoulderState = jointResult.EvaluationVerdict.ShoulderOk ? ParameterState.Good : ParameterState.Bad;
                }
                else
                {
                    ResultTorqueState = ParameterState.Normal;
                    ResultLengthState = ParameterState.Normal;
                    ResultShoulderState = ParameterState.Normal;
                }
            }
            else
            {
                ResultTotalState = ParameterState.Normal;
                ResultTorqueState = ParameterState.Normal;
                ResultLengthState = ParameterState.Normal;
                ResultShoulderState = ParameterState.Normal;
            }
        }
        #endregion
        private void ResetResultsState()
        {
            JointResult = null;
            ResultTotalState = ParameterState.Normal;
            ResultTorqueState = ParameterState.Normal;
            ResultLengthState = ParameterState.Normal;
            ResultShoulderState = ParameterState.Normal;
        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SecondsFromBeginJointing = (int)(DateTime.Now - _jointingStartTime).TotalSeconds;
        }

        private void InitializeActualPointTimer()
        {
            _actualPointUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(ACTUAL_POINT_UPDATE_INTERVAL)
            };
            _actualPointUpdateTimer.Tick += (s, e) => OnPropertyChanged(nameof(ActualPoint));            
        }
    }
}
