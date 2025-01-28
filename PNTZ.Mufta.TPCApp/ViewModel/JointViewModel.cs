using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;

using Promatis.Core.Logging;

using System;

using System.Collections.ObjectModel;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Toolkit.IO;
using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel(JointResultDpWorker resultWorker, RecipeToPlc recipeLoader, ILogger logger, ICliProgram cliProgram, RepositoryContext repo )
        {
            this.logger = logger;
            this.cliProgram = cliProgram;
            this.repo = repo;

            try
            {
                var config = XDocument.Load($"{AppInstance.CurrentDirectory}/ViewModel/JointViewModel.xml");
                UpdateInterval = TimeSpan.FromMilliseconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("UpdateInterval").Value));
                RecordingInterval = TimeSpan.FromMilliseconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("RecordingInterval").Value));
                MaxRecordingTime = TimeSpan.FromSeconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("MaxRecordingTimeSec").Value));

                TorqueTimeChartTorqueMaxValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("TorqueMaxVal").Value);

            }
            catch (Exception ex)
            {
                logger.Info("Не удалось загрузить конфигурацию для JointViewModel:");
                logger.Info(ex.Message);
                logger.Info("Будут использованы значения по-умолчанию");
            }

            //Настройки графиков
            OnPropertyChanged(nameof(TorqueTimeChartTorqueMaxValue));

            this.ResultDpWorker = resultWorker;

            this.RecipeLoader = recipeLoader;


            //Кнопки установки результата
            SetGoodResultCommand = new RelayCommand((arg) =>
            {
                resultWorker.Evaluate(1);
                ShowResultButtons = false;
                OnPropertyChanged(nameof(ShowResultButtons));
            });
            SetBadResultCommand = new RelayCommand((arg) =>
            {
                resultWorker.Evaluate(2);
                ShowResultButtons = false;
                OnPropertyChanged(nameof(ShowResultButtons));
            });
        }



        ILogger logger;
        ICliProgram cliProgram;

        RepositoryContext repo;
        

        //Класс получения параметров из OpcUa
        JointResultDpWorker resultWorker;
        JointResultDpWorker ResultDpWorker
        {
            get => resultWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                resultWorker = value;

                resultWorker.RecordingBegun += StartChartRecording;
                resultWorker.RecordingFinished += StopChartRecording;

                resultWorker.AwaitForEvaluation += (s, v) =>
                {
                    ShowResultButtons = true;
                    OnPropertyChanged(nameof(ShowResultButtons));
                };

                ResultDpWorker.DpParam.ValueUpdated += SubscribeToValues;

                ResultDpWorker.JointFinished += (s, v) => SetResult(v);                

                cliProgram.RegisterCommand("startjoint", (arg) => resultWorker.CyclicallyListen = true);
                cliProgram.RegisterCommand("stopjoint", (arg) => resultWorker.CyclicallyListen = false);
                ResultDpWorker.CyclicallyListen = true;
            }
        }

        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        public float ActualTorque { get; set; } = 0;
        public float ActualLength { get; set; } = 0;
        public float ActualTurns { get; set; } = 0;
        public float ActualTurnsPerMinute { get; set; } = 0;


        //Такс циклично обновляющий показания. Показания обновляются раз в заданный интервал
        private void SubscribeToValues(object sender, DpConnect.Struct.OperationalParam e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    ActualTorque = ResultDpWorker.DpParam.Value.Torque;
                    ActualLength = ResultDpWorker.DpParam.Value.Length;
                    ActualTurns = ResultDpWorker.DpParam.Value.Turns;
                    

                    OnPropertyChanged(nameof(ActualTorque));
                    OnPropertyChanged(nameof(ActualLength));
                    OnPropertyChanged(nameof(ActualTurns));

                    if (ActualTurns == 0)
                    {
                        ActualTurnsPerMinute = 0;
                        OnPropertyChanged(nameof(ActualTurnsPerMinute));
                    }

                    await Task.Delay(UpdateInterval);
                }
            });
            ResultDpWorker.DpParam.ValueUpdated -= SubscribeToValues;
        }


        //***************** ДАННЫЕ РЕЦЕПТА **********************
        RecipeToPlc recipeLoader;
        RecipeToPlc RecipeLoader
        {
            get => recipeLoader;
            set 
            {
                if (value == null)
                    throw new ArgumentNullException();

                recipeLoader = value;

                recipeLoader.RecipeLoaded += (s, v) =>
                {
                    LoadedRecipe = v;
                    OnPropertyChanged(nameof(LoadedRecipe));
                };
            }
        }
        
        public JointRecipe LoadedRecipe { get; set; }





        // ************** ЗАПИСЬ ГРАФИКОВ ***********************
        TimeSpan RecordingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        TimeSpan MaxRecordingTime { get; set; } = TimeSpan.FromSeconds(60);
        public ObservableCollection<TqTnLenPointViewModel> ChartSeries { get; set; }
        CancellationTokenSource RecordingCts;
        bool RecordingProcedureStarted = false;

        public TqTnLenPoint LastPoint { get; set; }
        //Вход в цикл записи графиков
        private async void StartChartRecording(object sender, EventArgs e)
        {
            if (RecordingProcedureStarted)
                throw new InvalidOperationException("Операция записи графиков уже начата");

            RecordingProcedureStarted = true;

            logger.Info("Начинаем запись графиков для UI...");            

            RecordingCts = new CancellationTokenSource();

            try
            {
                await RecordSeriesCycle(RecordingCts.Token);                
            }
            catch (TimeoutException ex)
            {
                logger.Info(ex.Message);
            }
            catch (OperationCanceledException)
            {
                logger.Info("Запись графиков для UI завершена");
            }
            catch (Exception ex)
            {
                logger.Info("Неизвестная ошибка во время записи графиков: " + ex.Message);
            }
            finally
            {
                RecordingProcedureStarted = false;
            }
        }
        //Таск цикл записи графиков
        private async Task RecordSeriesCycle(CancellationToken token)
        {
            ChartSeries = new ObservableCollection<TqTnLenPointViewModel>();
            OnPropertyChanged(nameof(ChartSeries));

            DateTime beginTime = DateTime.Now;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
                else
                {
                    TqTnLenPoint lastPoint = LastPoint;

                    TqTnLenPoint newPoint = new TqTnLenPoint()
                    {
                        Torque = ActualTorque,
                        Turns = ActualTurns,
                        Length = ActualLength,
                        TimeStamp = Convert.ToInt32(DateTime.Now.Subtract(beginTime).TotalMilliseconds)
                    };                    

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        CalculateTurnsPerMinute(lastPoint, newPoint);
                        LastPoint = newPoint;
                        OnPropertyChanged(nameof(LastPoint));
                        OnPropertyChanged(nameof(ActualTurnsPerMinute));

                        var newTq = new TqTnLenPointViewModel(LastPoint)
                        {
                            TurnsPerMinute = ActualTurnsPerMinute
                        };

                        ChartSeries.Add(newTq);
                        
                    });


                }
                await Task.Delay(RecordingInterval);
            }        
        }
        private void CalculateTurnsPerMinute(TqTnLenPoint lastPoint, TqTnLenPoint newPoint)
        {
            if (lastPoint == null || newPoint == null)
                return;

            const int millisecondsInMinute = 60_000;


            double dV = (newPoint.Turns - lastPoint.Turns);
            double dT = (newPoint.TimeStamp - lastPoint.TimeStamp);
            double dTminutes = dT / millisecondsInMinute;
            double changeRate = (dV / dTminutes);


            if (dTminutes > 0 && dV != 0)
                ActualTurnsPerMinute = (float)changeRate;

        }

        private void StopChartRecording(object sender, EventArgs e)
        {
            if (RecordingCts != null)
            {
                RecordingCts.Cancel();
                RecordingCts = null;
            }
        }

        //************** НАСТРОЙКА ГРАФИКОВ ***************************

        public double TorqueTimeChartTorqueMaxValue { get; set; } = 2000;



        // ************* УСТАНОВКА РЕЗУЛЬТАТА ******************

        public ICommand SetGoodResultCommand { get; private set; }
        public ICommand SetBadResultCommand { get; private set; }
        public bool ShowResultButtons { get; set; }


        // ************* РЕЗУЛЬТАТ ****************

        void SetResult(JointResult result)
        {
            ShowResultButtons = false;
            OnPropertyChanged(nameof(ShowResultButtons));

            result.Recipe = LoadedRecipe;

            try
            {
                repo.SaveResult(result);
                LastJointResult = result;
                OnPropertyChanged(nameof(LastJointResult));
            }
            catch (Exception ex)
            {
                logger.Info("Ошибка при сохранении записи " + ex);
            }
            


        }
        public JointResult LastJointResult { get; set; }
    }
}

