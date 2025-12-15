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
