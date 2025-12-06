using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PNTZ.Mufta.TPCApp.View.ControlHelper
{
    public static class GridHelper
    {
        // Включить/выключить границы
        public static readonly DependencyProperty ShowBordersProperty =
            DependencyProperty.RegisterAttached("ShowBorders", typeof(bool), typeof(GridHelper),
                new PropertyMetadata(false, OnShowBordersChanged));

        public static bool GetShowBorders(DependencyObject obj) => (bool)obj.GetValue(ShowBordersProperty);
        public static void SetShowBorders(DependencyObject obj, bool value) => obj.SetValue(ShowBordersProperty, value);

        // Цвет границ
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(GridHelper),
                new PropertyMetadata(Brushes.Gray));

        public static Brush GetBorderBrush(DependencyObject obj) => (Brush)obj.GetValue(BorderBrushProperty);
        public static void SetBorderBrush(DependencyObject obj, Brush value) => obj.SetValue(BorderBrushProperty, value);

        // Толщина границ
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(double), typeof(GridHelper),
                new PropertyMetadata(1.0));

        public static double GetBorderThickness(DependencyObject obj) => (double)obj.GetValue(BorderThicknessProperty);
        public static void SetBorderThickness(DependencyObject obj, double value) => obj.SetValue(BorderThicknessProperty, value);

        private static void OnShowBordersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid && (bool)e.NewValue)
            {
                grid.Loaded += (s, args) => DrawBorders(grid);
            }
        }

        private static void DrawBorders(Grid grid)
        {
            var brush = GetBorderBrush(grid);
            var thickness = GetBorderThickness(grid);

            // Вертикальные линии (между колонками)
            for (int i = 1; i < grid.ColumnDefinitions.Count; i++)
            {
                var line = new Rectangle
                {
                    Fill = brush,
                    Width = thickness,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                Grid.SetColumn(line, i);
                Grid.SetRowSpan(line, grid.RowDefinitions.Count);
                grid.Children.Add(line);
            }

            // Горизонтальные линии (между строками)
            for (int i = 1; i < grid.RowDefinitions.Count; i++)
            {
                var line = new Rectangle
                {
                    Fill = brush,
                    Height = thickness,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top
                };
                Grid.SetRow(line, i);
                Grid.SetColumnSpan(line, grid.ColumnDefinitions.Count);
                grid.Children.Add(line);
            }
        }
    }
}