using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.View
{
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached(
                "Watermark",
                typeof(string),
                typeof(WatermarkService),
                new PropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);

        public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Hello from WatermarkService.OnWatermarkChanged! " + d.GetType().Name + " " + e.NewValue);
            if (d is TextBox textBox)
            {
                textBox.Loaded += (s, ev) => AddWatermark(textBox);
                textBox.TextChanged += (s, ev) => AddWatermark(textBox);
            }
        }

        private static void AddWatermark(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var adorners = layer.GetAdorners(textBox);
            if (string.IsNullOrEmpty(textBox.Text))
            {
                if (adorners == null)
                {
                    layer.Add(new WatermarkAdorner(textBox, GetWatermark(textBox)));
                }
                else
                {
                    bool hasWatermark = false;
                    foreach (var adorner in adorners)
                    {
                        if (adorner is WatermarkAdorner)
                        {
                            hasWatermark = true;
                            break;
                        }
                    }
                    if (!hasWatermark)
                        layer.Add(new WatermarkAdorner(textBox, GetWatermark(textBox)));
                }
            }
            else
            {
                if (adorners == null) return;
                foreach (var adorner in adorners)
                {
                    if (adorner is WatermarkAdorner)
                    {
                        layer.Remove(adorner);
                    }
                }
            }
        }
    }
}
