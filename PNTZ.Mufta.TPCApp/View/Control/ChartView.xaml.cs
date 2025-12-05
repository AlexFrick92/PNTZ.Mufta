using PNTZ.Mufta.TPCApp.ViewModel.Control;
using System.Windows;
using System.Windows.Controls;

namespace PNTZ.Mufta.TPCApp.View.Control
{
    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();

            // Подписываемся на изменение DataContext для обработки ResetZoomTrigger
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Отписываемся от старого ViewModel
            if (e.OldValue is ChartViewModel oldVm)
            {
                oldVm.PropertyChanged -= OnViewModelPropertyChanged;
            }

            // Подписываемся на новый ViewModel
            if (e.NewValue is ChartViewModel newVm)
            {
                newVm.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChartViewModel.ResetZoomTrigger))
            {
                //ResetZoom();
            }
        }

        /// <summary>
        /// Метод для сброса зума графика
        /// </summary>
        private void ResetZoom()
        {
            diagram.ActualAxisX?.ActualVisualRange?.SetAuto();
            diagram.ActualAxisY?.ActualVisualRange?.SetAuto();
        }
    }
}
