using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using PNTZ.Mufta.TPCApp.ViewModel.Joint;

namespace PNTZ.Mufta.TPCApp.View.Joint
{
    /// <summary>
    /// Interaction logic for JointResultAnalysisView.xaml
    /// </summary>
    public partial class JointResultAnalysisView : UserControl
    {
        private JointResultAnalysisViewModel ViewModel => DataContext as JointResultAnalysisViewModel;

        public JointResultAnalysisView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Обработчик изменения чекбоксов видимости элементов визуализации
        /// </summary>
        private void ChkVisibility_Changed(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                return;

            // Обновляем флаги видимости в ViewModel
            ViewModel.ShowSmoothedTorque = ChkShowSmoothedTorque?.IsChecked ?? true;
            ViewModel.ShowDerivative = ChkShowDerivative?.IsChecked ?? true;
            ViewModel.ShowSigmaLines = ChkShowSigmaLines?.IsChecked ?? true;
            ViewModel.ShowThreshold = ChkShowThreshold?.IsChecked ?? true;
            ViewModel.ShowBaseline = ChkShowBaseline?.IsChecked ?? true;
        }

        /// <summary>
        /// Обработчик кнопки "Выполнить расчёт" детектора заплечника
        /// </summary>
        private void BtnRunDetection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    DetectionResultText.Text = "⚠ ViewModel не инициализирован";
                    return;
                }

                // Проверяем, что результат загружен
                if (ViewModel.CurrentResult == null)
                {
                    DetectionResultText.Text = "⚠ Результат не загружен";
                    return;
                }

                // Читаем параметры из UI
                if (!int.TryParse(TxtWindowSize.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int windowSize) || windowSize <= 0)
                {
                    DetectionResultText.Text = "⚠ Неверное значение WindowSize";
                    return;
                }

                if (!int.TryParse(TxtDerivativeWindowSize.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int derivativeWindowSize) || derivativeWindowSize <= 0)
                {
                    DetectionResultText.Text = "⚠ Неверное значение DerivativeWindowSize";
                    return;
                }

                if (!double.TryParse(TxtSigmaMultiplier.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double sigmaMultiplier) || sigmaMultiplier <= 0)
                {
                    DetectionResultText.Text = "⚠ Неверное значение SigmaMultiplier";
                    return;
                }

                if (!double.TryParse(TxtStartWindow.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double searchStartRatio) 
                    || searchStartRatio <= 0 || searchStartRatio > 1)
                {
                    DetectionResultText.Text = "⚠ Неверное значение окна поиска";
                    return;
                }                

                // Передаём параметры в ViewModel
                ViewModel.WindowSize = windowSize;
                ViewModel.DerivativeWindowSize = derivativeWindowSize;
                ViewModel.SigmaMultiplier = sigmaMultiplier;
                ViewModel.SearchStartRatio = searchStartRatio;

                DetectionResultText.Text = "⏳ Выполняется расчёт...";

                // Запускаем детектор
                ViewModel.RunShoulderDetection();

                // Получаем результат из последнего расчёта
                var result = ViewModel.GetLastDetectionResult();

                if (result?.ShoulderPointIndex.HasValue == true)
                {
                    int index = result.ShoulderPointIndex.Value;
                    var point = ViewModel.CurrentResult.Series[index];

                    DetectionResultText.Text = $"✓ Точка найдена!\n" +
                        $"Индекс: {index}\n" +
                        $"Момент: {point.Torque:F1} Nm\n" +
                        $"Обороты: {point.Turns:F3}\n" +
                        $"Время: {point.TimeStamp / 1000.0:F2} сек";
                }
                else
                {
                    DetectionResultText.Text = "✗ Точка заплечника не найдена";
                }
            }
            catch (Exception ex)
            {
                DetectionResultText.Text = $"⚠ Ошибка: {ex.Message}";
            }
        }
    }
}
