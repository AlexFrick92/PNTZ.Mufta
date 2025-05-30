using Desktop.MVVM;
using DevExpress.Xpf.Charts;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect;
using PNTZ.Mufta.TPCApp.Repository;
using PNTZ.Mufta.TPCApp.View;
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

                jointProcessWorker.AwaitForEvaluation += (s, v) =>
                {
                    ShowResultButtons = true;
                    OnPropertyChanged(nameof(ShowResultButtons));
                };

                JointProcessWorker.NewTqTnLenPoint+= SubscribeToValues;

                JointProcessWorker.JointFinished += (s, v) => SetResult(v);                

                cliProgram.RegisterCommand("startjoint", (arg) => jointProcessWorker.CyclicallyListen = true);
                cliProgram.RegisterCommand("stopjoint", (arg) => jointProcessWorker.CyclicallyListen = false);
                JointProcessWorker.CyclicallyListen = true;
            }
        }

        public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        
        //Актуальные показания с датчиков

        public TqTnLenPointViewModel ActualPoint { get; set; }
        //Сглаженные актуальные показаний с датчиков
        public float ActualTorqueSmoothed { get; set; } = 0;


        //Такс циклично обновляющий показания. Показания обновляются раз в заданный интервал
        private void SubscribeToValues(object sender, TqTnLenPoint e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var point = new TqTnLenPointViewModel(e);
                    ActualPoint = point;

                    OnPropertyChanged(nameof(ActualPoint));

                    await Task.Delay(UpdateInterval);
                }
            });
            JointProcessWorker.NewTqTnLenPoint -= SubscribeToValues;
        }

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
            OnPropertyChanged(nameof(TorqueTurnsChartConfig));

            //Обороты/мин/обороты
            TurnsPerMinuteTurnsChartConfig.XMinValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("XMin").Value);
            TurnsPerMinuteTurnsChartConfig.XMaxValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("XMax").Value);
            TurnsPerMinuteTurnsChartConfig.YMinValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("YMin").Value);
            TurnsPerMinuteTurnsChartConfig.YMaxValue = double.Parse(config.Root.Element("TurnsPerMinuteTurnsChart").Attribute("YMax").Value);
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChartConfig));

            //Момент/Время
            TorqueTimeChartConfig.XMinValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("XMin").Value);
            TorqueTimeChartConfig.XMaxValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("XMax").Value);
            TorqueTimeChartConfig.YMinValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("YMin").Value);
            TorqueTimeChartConfig.YMaxValue = double.Parse(config.Root.Element("TorqueTimeChart").Attribute("YMax").Value);
            OnPropertyChanged(nameof(TorqueTimeChartConfig));

            //Момент/длина
            TorqueLengthChartConfig.XMinValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("XMin").Value);
            TorqueLengthChartConfig.XMaxValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("XMax").Value);
            TorqueLengthChartConfig.YMinValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("YMin").Value);
            TorqueLengthChartConfig.YMaxValue = double.Parse(config.Root.Element("TorqueLengthChart").Attribute("YMax").Value);
            OnPropertyChanged(nameof(TorqueLengthChartConfig));
        }

        private void ChartConfigByRecipe(JointRecipe recipe)
        {
            TorqueLengthChartConfig.YMinValue = 0;
            TorqueLengthChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            TorqueLengthChartConfig.XMinValue = recipe.MU_Len_Min / 1.2;
            TorqueLengthChartConfig.XMaxValue = recipe.MU_Len_Max * 1.05;

            OnPropertyChanged(nameof(TorqueLengthChartConfig));

            TorqueTurnsChartConfig.YMinValue = 0;
            TorqueTurnsChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            TorqueTurnsChartConfig.XMaxValue = recipe.MU_Len_Max / recipe.Thread_step;
            OnPropertyChanged(nameof(TorqueTurnsChartConfig));


            TorqueTimeChartConfig.YMinValue = 0;
            TorqueTimeChartConfig.YMaxValue = recipe.MU_Tq_Max * 1.1;
            OnPropertyChanged(nameof(TorqueTimeChartConfig));

            TurnsPerMinuteTurnsChartConfig.XMaxValue = recipe.MU_Len_Max / recipe.Thread_step;
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChartConfig));
        }

        private void ChartConfigByPreJointDataData(object sender, JointResult result)
        {
            TorqueLengthChartConfig.XMinValue = result.MVS_Len;
            OnPropertyChanged(nameof(TorqueLengthChartConfig));
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
                recordingProcedureStarted = false;
            }
        }
        //Таск цикл записи графиков
        private async Task RecordSeriesCycle(CancellationToken token)
        {
            ChartSeries = new ObservableCollection<TqTnLenPointViewModel>();
            ChartSeriesSmoothed = new ObservableCollection<TqTnLenPointViewModel>();

            OnPropertyChanged(nameof(ChartSeries));
            OnPropertyChanged(nameof(ChartSeriesSmoothed));

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
                        ChartSeries.Add(ActualPoint);
                    });


                }
                await Task.Delay(RecordingInterval);
            }        
        }
        private void StopChartRecording(object sender, EventArgs e)
        {
            if (recordingCts != null)
            {
                recordingCts.Cancel();
                recordingCts = null;
            }
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

