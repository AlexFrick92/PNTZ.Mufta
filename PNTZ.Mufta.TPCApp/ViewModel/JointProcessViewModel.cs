using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View;
using Promatis.Core.Logging;

using System.Reactive.Subjects;
using System.Reactive.Linq;

using System;

using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using LinqToDB.Tools;
using Toolkit.IO;
using static PNTZ.Mufta.TPCApp.App;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointProcessViewModel : BaseViewModel
    {
        public JointProcessViewModel(IJointProcessWorker jointProcessWorker, IRecipeLoader recipeLoader, ILogger logger, ICliProgram cliProgram, RepositoryContext repo )
        {
            this.logger = logger;
            this.cliProgram = cliProgram;
            this.repo = repo;

            try
            {
                var config = XDocument.Load($"{AppInstance.CurrentDirectory}/ViewModel/JointProcessViewModel.xml");
                UpdateInterval = TimeSpan.FromMilliseconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("UpdateInterval").Value));
                RecordingInterval = TimeSpan.FromMilliseconds(int.Parse(config.Root.Element("JointOperationParam").Attribute("RecordingInterval").Value));

                InitChartConfig(config);
            }
            catch (Exception ex)
            {
                logger.Info("Не удалось загрузить конфигурацию для JointProcessViewModel:");
                logger.Info(ex.Message);
                logger.Info("Будут использованы значения по-умолчанию");
            }

            this.JointProcessWorker = jointProcessWorker;

            this.RecipeLoader = recipeLoader;


            //Кнопки установки результата
            SetGoodResultCommand = new RelayCommand((arg) =>
            {
                jointProcessWorker.Evaluate(1);
                ShowResultButtons = false;
                OnPropertyChanged(nameof(ShowResultButtons));
            });
            SetBadResultCommand = new RelayCommand((arg) =>
            {
                jointProcessWorker.Evaluate(2);
                ShowResultButtons = false;
                OnPropertyChanged(nameof(ShowResultButtons));
            });
        }


        ILogger logger;
        ICliProgram cliProgram;

        RepositoryContext repo;

        //Класс получения параметров из OpcUa
        IJointProcessWorker jointProcessWorker;
        IJointProcessWorker JointProcessWorker
        {
            get => jointProcessWorker;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                jointProcessWorker = value;


                jointProcessWorker.PipeAppear += ChartConfigByPreJointDataData; 

                jointProcessWorker.RecordingBegun += StartChartRecording;
                jointProcessWorker.RecordingFinished += StopChartRecording;
                jointProcessWorker.RecordingFinished += ShowResultAfterRecording;

                jointProcessWorker.AwaitForEvaluation += (s, v) =>
                {
                    ShowResultButtons = true;
                    OnPropertyChanged(nameof(ShowResultButtons));
                };

                JointProcessWorker.NewTqTnLenPoint+= (s, e) => _actualPointStream.OnNext(e);

                _actualPointStream
                    //.Buffer(UpdateInterval)
                    //.Where(buf => buf.Count > 0)
                    //.Select(TqTnLenPoint.SmoothAverage)
                    .Sample(UpdateInterval)
                    .Subscribe(val =>
                    {
                        ActualPoint = new TqTnLenPointViewModel(val);
                        OnPropertyChanged(nameof(ActualPoint));
                    });


                JointProcessWorker.JointFinished += (s, v) => SetResult(v);                

                cliProgram.RegisterCommand("startjoint", (arg) => jointProcessWorker.CyclicallyListen = true);
                cliProgram.RegisterCommand("stopjoint", (arg) => jointProcessWorker.CyclicallyListen = false);
                JointProcessWorker.CyclicallyListen = true;
            }
        }



        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        //Актуальные показания с датчиков


        private Subject<TqTnLenPoint> _actualPointStream = new Subject<TqTnLenPoint>();

        public TqTnLenPointViewModel ActualPoint { get; set; }

        //Сглаженные актуальные показаний с датчиков
        public float ActualTorqueSmoothed { get; set; } = 0;

        //***************** ДАННЫЕ РЕЦЕПТА **********************
        IRecipeLoader recipeLoader;
        IRecipeLoader RecipeLoader
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
        JointRecipe loadedRecipe;
        public JointRecipe LoadedRecipe 
        { 
            get => loadedRecipe;
            set
            {
                loadedRecipe = value;
                jointProcessWorker.SetActualRecipe(value);
                ChartConfigByRecipe(value);
            } 
        }

        private void InitChartConfig(XDocument config)
        {
            //Первичная настройка графиков

            //Момент/Обороты
            TorqueTurnsChartConfig.XMinValue = double.Parse(config.Root.Element("TorqueTurnsChart").Attribute("XMin").Value);
            TorqueTurnsChartConfig.XMaxValue = double.Parse(config.Root.Element("TorqueTurnsChart").Attribute("XMax").Value);
            TorqueTurnsChartConfig.YMinValue = double.Parse(config.Root.Element("TorqueTurnsChart").Attribute("YMin").Value);
            TorqueTurnsChartConfig.YMaxValue = double.Parse(config.Root.Element("TorqueTurnsChart").Attribute("YMax").Value);

            //Обороты/мин/обороты
            TurnsPerMinuteTurnsChartConfig.XMinValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("XMin").Value);
            TurnsPerMinuteTurnsChartConfig.XMaxValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("XMax").Value);
            TurnsPerMinuteTurnsChartConfig.YMinValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("YMin").Value);
            TurnsPerMinuteTurnsChartConfig.YMaxValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("YMax").Value);

            //Момент/Время
            TorqueTimeChartConfig.XMinValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("XMin").Value);
            TorqueTimeChartConfig.XMaxValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("XMax").Value);
            TorqueTimeChartConfig.YMinValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("YMin").Value);
            TorqueTimeChartConfig.YMaxValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("YMax").Value);

            //Момент/длина
            TorqueLengthChartConfig.XMinValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("XMin").Value);
            TorqueLengthChartConfig.XMaxValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("XMax").Value);
            TorqueLengthChartConfig.YMinValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("YMin").Value);
            TorqueLengthChartConfig.YMaxValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("YMax").Value);
            
        }

        private void ChartConfigByRecipe(JointRecipe recipe)
        {
            TorqueLengthChartConfig.YMinValue = 0;
            TorqueLengthChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            TorqueLengthChartConfig.XMinValue = recipe.MU_Len_Min / 1.2;
            TorqueLengthChartConfig.XMaxValue = recipe.MU_Len_Max * 1.05;

            TorqueTurnsChartConfig.YMinValue = 0;
            TorqueTurnsChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            TorqueTurnsChartConfig.XMaxValue = recipe.MU_Len_Max / recipe.Thread_step;
            
            TorqueTimeChartConfig.YMinValue = 0;
            TorqueTimeChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            
            TurnsPerMinuteTurnsChartConfig.XMaxValue = recipe.MU_Len_Max / recipe.Thread_step;
            
        }

        private void ChartConfigByPreJointDataData(object sender, JointResult result)
        {
            var resultvm = new JointResultViewModel(result);
            TorqueLengthChartConfig.XMinValue = resultvm.MVS_Len;
        }


        // ************** ЗАПИСЬ ГРАФИКОВ ***********************
        TimeSpan RecordingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        public ObservableCollection<TqTnLenPointViewModel> ChartSeries { get; set; }
        public ObservableCollection<TqTnLenPointViewModel> ChartSeriesSmoothed { get; set; }

        CancellationTokenSource recordingCts;
        bool recordingProcedureStarted = false;
        //Вход в цикл записи графиков
        private async void StartChartRecording(object sender, EventArgs e)
        {
            if (recordingProcedureStarted)
                throw new InvalidOperationException("Операция записи графиков уже начата");

            recordingProcedureStarted = true;

            logger.Info("Начинаем запись графиков для UI...");            

            recordingCts = new CancellationTokenSource();

            try
            {
                await RecordSeriesCycle(recordingCts.Token);                
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
                _graphSubscription?.Dispose();
                recordingProcedureStarted = false;
            }
        }
        //Таск цикл записи графиков
        private IDisposable _graphSubscription;
        private async Task RecordSeriesCycle(CancellationToken token)
        {
            ChartSeries = new ObservableCollection<TqTnLenPointViewModel>();
            ChartSeriesSmoothed = new ObservableCollection<TqTnLenPointViewModel>();

            OnPropertyChanged(nameof(ChartSeries));
            OnPropertyChanged(nameof(ChartSeriesSmoothed));


            _graphSubscription = _actualPointStream
                //.Buffer(UpdateInterval)
                //.Where(buf => buf.Count > 0)
                //.Select(TqTnLenPoint.SmoothAverage)
                .Sample(UpdateInterval)
                .ObserveOn(System.Windows.Application.Current.Dispatcher)
                .Subscribe(val =>
                {
                    ChartSeries.Add(new TqTnLenPointViewModel(val));
                    
                });

            await Task.Delay(Timeout.Infinite, token);


        }
        private void StopChartRecording(object sender, JointResult e)
        {
            if (recordingCts != null)
            {
                recordingCts.Cancel();
                recordingCts = null;
            }
        }
        private void ShowResultAfterRecording(object sender, JointResult e)
        {
            LastJointResult = new JointResultViewModel(e);
            OnPropertyChanged(nameof(LastJointResult));
        }

        // ************* УСТАНОВКА РЕЗУЛЬТАТА ******************

        public ICommand SetGoodResultCommand { get; private set; }
        public ICommand SetBadResultCommand { get; private set; }
        public bool ShowResultButtons { get; set; }


        // ************* РЕЗУЛЬТАТ ****************

        void SetResult(JointResult result)
        {
            ShowResultButtons = false;
            OnPropertyChanged(nameof(ShowResultButtons));

            try
            {
                repo.SaveResult(result);
                LastJointResult = new JointResultViewModel(result);
                OnPropertyChanged(nameof(LastJointResult));
            }
            catch (Exception ex)
            {
                logger.Info("Ошибка при сохранении записи " + ex);
            }
            


        }
        public JointResultViewModel LastJointResult { get; set; }



        // ************* НАСТРОЙКА ГРАФИКОВ ******************

        public ChartViewConfig TorqueTimeChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TorqueLengthChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TurnsPerMinuteTurnsChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TorqueTurnsChartConfig { get; set; } = new ChartViewConfig();

    }
}

