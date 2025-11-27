using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace PNTZ.Mufta.TPCApp.View.Control
{
    public class ConstantLineModel : Freezable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected override Freezable CreateInstanceCore() => new ConstantLineModel();


        // Value
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(ConstantLineModel),
                new PropertyMetadata(0.0, (d, e) =>
                {
                    ((ConstantLineModel)d).OnPropertyChanged(nameof(Value));
                    //Console.WriteLine("New value set: {0}", e.NewValue);
                }));


        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);           
        }

        // Label
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(ConstantLineModel),
                new PropertyMetadata(string.Empty));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        // Color
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(SolidColorBrush),
                typeof(ConstantLineModel),
                new PropertyMetadata(Brushes.Red));

        public SolidColorBrush Color
        {
            get => (SolidColorBrush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public ConstantLineModel() { }
    }
}
