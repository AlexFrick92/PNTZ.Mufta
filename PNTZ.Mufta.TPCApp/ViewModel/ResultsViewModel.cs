using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using Promatis.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PNTZ.Mufta.TPCApp.View;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class ResultsViewModel : BaseViewModel
    {
        private readonly LocalRepositoryContext _repo;
        private readonly ILogger _logger;
        public ICommand GetResultCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ResultsViewModel(LocalRepositoryContext repo, ILogger logger)
        {
            _repo = repo;

            _logger = logger;

            GetResultCommand = new RelayCommand((arg) =>
            {
                Results = new ObservableCollection<JointResultViewModel>();

                try
                {
                    Results.AddRange(repo.ResultsTable
                        .Where(t => t.Name == SelectedRecipeName)
                        .Select(t => new JointResultViewModel(t.ToJointResult())));                    
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error loading results for recipe '{SelectedRecipeName}': {ex.Message}", ex);
                }

                //Results.AddRange(repo.LoadResults().Select(r => new JointResultViewModel(r)));
                OnPropertyChanged(nameof(Results));
            });


            RefreshCommand = new RelayCommand((arg) =>
            {
                FilteredRecipeNames = new ObservableCollection<string>();
                FilteredRecipeNames.AddRange(repo.ResultsTable.Select(t => t.Name).Distinct());
                OnPropertyChanged(nameof(FilteredRecipeNames));
            });
            
            RefreshCommand.Execute(null);   
           
        }


        //------------------------------------------------
        
        private string _selectedRecipeName;
        public ObservableCollection<string> FilteredRecipeNames { get; private set; } = new ObservableCollection<string>();
        public string SelectedRecipeName
        {
            get => _selectedRecipeName;
            set
            {
                if (_selectedRecipeName == value) return;
                _selectedRecipeName = value;
                OnPropertyChanged(nameof(SelectedRecipeName));
                GetResultCommand.Execute(null);
                if (!string.IsNullOrWhiteSpace(_selectedRecipeName))
                    _logger.Info($"Selected recipe: {_selectedRecipeName}");
                else
                    _logger.Info("SelectedRecipeName was reset to null or empty");
            }
        }

        //------------------------------------------------

        public ChartViewConfig TorqueTimeChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TorqueLengthChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TurnsPerMinuteTurnsChartConfig { get; set; } = new ChartViewConfig();
        public ChartViewConfig TorqueTurnsChartConfig { get; set; } = new ChartViewConfig();

        public ObservableCollection<JointResultViewModel> Results { get; set;  }

        JointResultViewModel selectedResult;
        public JointResultViewModel SelectedResult
        {
            get => selectedResult;
            set
            {
                if (value != null)
                {
                    ResultSelected = true;
                    OnPropertyChanged(nameof(ResultSelected));
                    selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));

                    SetChartsConfig(value);
                }
                else
                {
                    ResultSelected = false;
                    OnPropertyChanged(nameof(ResultSelected));
                }
            }

        }

        public bool ResultSelected { get; set; }

        private void SetChartsConfig(JointResultViewModel result)
        {
            if (result.Series.Count < 1)
            {
                _logger.Error("Selected result has no series to display in charts.");
                return;
            }

            var series = result.Series;

            //Момент/Обороты
            var torqueTurnsChartConfig = new ChartViewConfig()
            {
                XMinValue = series.First().Turns,
                XMaxValue = series.Last().Turns,
                YMinValue = series.Min(x => x.Torque),
                YMaxValue = series.Max(x => x.Torque),

            };
            TorqueTurnsChartConfig = torqueTurnsChartConfig;
            OnPropertyChanged(nameof(TorqueTurnsChartConfig));

            //Обороты/мин/обороты

            var turnsPerMinuteTurnsChartConfig = new ChartViewConfig()
            {
                XMinValue = series.First().Turns,
                XMaxValue = series.Last().Turns,
                YMinValue = series.Min(x => x.TurnsPerMinute),
                YMaxValue = series.Max(x => x.TurnsPerMinute),
            };
            TurnsPerMinuteTurnsChartConfig = turnsPerMinuteTurnsChartConfig;
            OnPropertyChanged(nameof(TurnsPerMinuteTurnsChartConfig));

            //Момент/Время

            var torqueTimeChartConfig = new ChartViewConfig()
            {
                XMinValue = series.First().TimeStamp,
                XMaxValue = series.Last().TimeStamp,
                YMinValue = series.Min(x => x.Torque),
                YMaxValue = series.Max(x => x.Torque),
            };
            TorqueTimeChartConfig = torqueTimeChartConfig;
            OnPropertyChanged(nameof(TorqueTimeChartConfig));
            
            //Момент/длина
            var torqueLengthChartConfig = new ChartViewConfig()
            {
                XMinValue = series.First().Length,
                XMaxValue = series.Last().Length,
                YMinValue = series.Min(x => x.Torque),
                YMaxValue = series.Max(x => x.Torque),
            };
            TorqueLengthChartConfig = torqueLengthChartConfig;
            OnPropertyChanged(nameof(TorqueLengthChartConfig));
        }
    }
}
