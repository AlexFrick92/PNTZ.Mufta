using Desktop.MVVM;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;

using Promatis.Core.Logging;

using System;

using System.Collections.ObjectModel;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointViewModel : BaseViewModel
    {
        public JointViewModel(JointOperationalParamDpWorker paramWorker, JointResultDpWorker resultWorker, RecipeToPlc recipeLoader, ILogger logger)
        {
            this.logger = logger;

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


            this.ParamWorker = paramWorker;

            this.ResultDpWorker = resultWorker;

            this.RecipeLoader = recipeLoader;

            SetGoodResultCommand = new RelayCommand((arg) =>
            {
                ShowResultButtons = false;
                OnPropertyChanged(nameof(ShowResultButtons));
            });
        }
        ILogger logger;
        



        // ************** ОБНОВЛЕНИЕ ЗНАЧЕНИЙ ***********************
        JointOperationalParamDpWorker jointOperationalParam;
        JointOperationalParamDpWorker ParamWorker

        {
            get => jointOperationalParam;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                jointOperationalParam = value;

                jointOperationalParam.DpParam.ValueUpdated += SubscribeToValues;
            }
        }
        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        public float ActualTorque { get; set; } = 0;
        public float ActualLength { get; set; } = 0;
        public float ActualTurns { get; set; } = 0;
        private void SubscribeToValues(object sender, DpConnect.Struct.OperationalParam e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    ActualTorque = jointOperationalParam.DpParam.Value.Torque;
                    ActualLength = jointOperationalParam.DpParam.Value.Length;
                    ActualTurns = jointOperationalParam.DpParam.Value.Turns;

                    OnPropertyChanged(nameof(ActualTorque));
                    OnPropertyChanged(nameof(ActualLength));
                    OnPropertyChanged(nameof(ActualTurns));

                    await Task.Delay(UpdateInterval);
                }
            });
            jointOperationalParam.DpParam.ValueUpdated -= SubscribeToValues;
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

        JointResultDpWorker resultWorker;
        JointResultDpWorker ResultDpWorker
        {
            get => resultWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                resultWorker = value;

                resultWorker.JointBegun += StartChartRecording;
                resultWorker.JointFinished += StopChartRecording;

                resultWorker.JointFinished += (s, v) =>
                {
                    ShowResultButtons = true;
                    OnPropertyChanged(nameof(ShowResultButtons));
                };
            }
        }
        TimeSpan RecordingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        TimeSpan MaxRecordingTime { get; set; } = TimeSpan.FromSeconds(60);
        public ObservableCollection<TqTnLenPoint> ChartSeries { get; set; }
        CancellationTokenSource RecordingCts;
        bool RecordingProcedureStarted = false;
        private async void StartChartRecording(object sender, EventArgs e)
        {
            if (RecordingProcedureStarted)
                throw new InvalidOperationException("Операция записи графиков уже начата");

            RecordingProcedureStarted = true;

            logger.Info("Начинаем запись графиков для UI...");            

            RecordingCts = new CancellationTokenSource();

            try
            {

                var timeout = Task.Delay(MaxRecordingTime);

                Task first = await Task.WhenAny(RecordSeriesCycle(RecordingCts.Token), timeout);

                await first;
                if (first == timeout)
                {
                    StopChartRecording(this, EventArgs.Empty);
                    throw new TimeoutException("Превышено максимальное время записи параметров.");
                }

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
        private async Task RecordSeriesCycle(CancellationToken token)
        {
            ChartSeries = new ObservableCollection<TqTnLenPoint>();
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
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ChartSeries.Add(new TqTnLenPoint()
                        {
                            Torque = ActualTorque,
                            Turns = ActualTurns,
                            Length = ActualLength,
                            TimeStamp = Convert.ToInt32(DateTime.Now.Subtract(beginTime).TotalMilliseconds)
                        });
                    });
                }
                await Task.Delay(RecordingInterval);
            }        
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
        public JointResult LastJointResult { get; set; }
    }
}

