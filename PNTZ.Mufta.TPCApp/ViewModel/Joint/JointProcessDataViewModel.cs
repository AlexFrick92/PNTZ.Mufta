using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.View.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    /// <summary>
    /// ViewModel для отображения данных процесса свинчивания
    /// </summary>
    public class JointProcessDataViewModel : BaseViewModel
    {
        //Текущие показания датчиков
        private TqTnLenPoint _actualPoint;
        public TqTnLenPoint ActualPoint
        {
            get { return _actualPoint; }
            set { _actualPoint = value; OnPropertyChanged(nameof(ActualPoint)); }
        }
        //Загруженный рецепт
        private JointRecipe _loadedRecipe;
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
        //Результат
        private JointResult _jointResult;
        public JointResult JointResult
        {
            get { return _jointResult; }
            set { _jointResult = value; OnPropertyChanged(nameof(JointResult)); }
        }
        //Время в секундах с начала процесса стыковки
        private int _secondsFromBeginJointing;
        public int SecondsFromBeginJointing
        {
            get { return _secondsFromBeginJointing; }
            set { _secondsFromBeginJointing = value; OnPropertyChanged(nameof(SecondsFromBeginJointing)); }
        }
        //Статус оценки результата
        private ParameterState _resultTotalState;
        public ParameterState ResultTotalState
        {
            get { return _resultTotalState; }
            set { _resultTotalState = value; OnPropertyChanged(nameof(ResultTotalState)); }
        }

        private ParameterState _resultTorqueState;
        public ParameterState ResultTorqueState
        {
            get { return _resultTorqueState; }
            set { _resultTorqueState = value; OnPropertyChanged(nameof(ResultTorqueState)); }
        }

        private ParameterState _resultLengthState;
        public ParameterState ResultLengthState
        {
            get { return _resultLengthState; }
            set { _resultLengthState = value; OnPropertyChanged(nameof(ResultLengthState)); }
        }

        private ParameterState _resultShoulderState;
        public ParameterState ResultShoulderState
        {
            get { return _resultShoulderState; }
            set { _resultShoulderState = value; OnPropertyChanged(nameof(ResultShoulderState)); }
        }

        public bool IsLengthMode => LoadedRecipe != null && (LoadedRecipe.JointMode == JointMode.Length || LoadedRecipe.JointMode == JointMode.TorqueLength);
        public bool IsShoulderMode => LoadedRecipe != null && LoadedRecipe.JointMode == JointMode.TorqueShoulder;

        //Таймер времени свинчивания
        private DispatcherTimer _timer;
        private DateTime _jointingStartTime;        
        public void UpdateRecipe(JointRecipe jointRecipe)
        {
            LoadedRecipe = jointRecipe;
        }
        public void BeginNewJointing()
        {
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateTimer_Tick;
            _jointingStartTime = DateTime.Now;
            SecondsFromBeginJointing = 0;
            _timer.Start();
            JointResult = null;
            ResultTotalState = ParameterState.Normal;
            ResultTorqueState = ParameterState.Normal;
            ResultLengthState = ParameterState.Normal;
            ResultShoulderState = ParameterState.Normal;
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
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SecondsFromBeginJointing = (int)(DateTime.Now - _jointingStartTime).TotalSeconds;
        }
    }
}
