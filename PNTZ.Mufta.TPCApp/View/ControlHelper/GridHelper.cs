using System;
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

        // Показывать внешнюю рамку
        public static readonly DependencyProperty ShowOutlineProperty =
            DependencyProperty.RegisterAttached("ShowOutline", typeof(bool), typeof(GridHelper),
                new PropertyMetadata(true));

        public static bool GetShowOutline(DependencyObject obj) => (bool)obj.GetValue(ShowOutlineProperty);
        public static void SetShowOutline(DependencyObject obj, bool value) => obj.SetValue(ShowOutlineProperty, value);

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
            var showOutline = GetShowOutline(grid);

            int rowCount = grid.RowDefinitions.Count;
            int colCount = grid.ColumnDefinitions.Count;

            // Случай: нет ни строк, ни столбцов - рисуем только внешнюю рамку (если showOutline = true)
            if (rowCount == 0 && colCount == 0)
            {
                if (showOutline)
                {
                    // Верхняя граница
                    grid.Children.Add(new Rectangle
                    {
                        Fill = brush,
                        Height = thickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top
                    });

                    // Нижняя граница
                    grid.Children.Add(new Rectangle
                    {
                        Fill = brush,
                        Height = thickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Bottom
                    });

                    // Левая граница
                    grid.Children.Add(new Rectangle
                    {
                        Fill = brush,
                        Width = thickness,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch
                    });

                    // Правая граница
                    grid.Children.Add(new Rectangle
                    {
                        Fill = brush,
                        Width = thickness,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Stretch
                    });
                }

                return;
            }

            // Горизонтальные линии
            if (rowCount == 0)
            {
                // Только верхняя и нижняя границы (если showOutline = true)
                if (showOutline)
                {
                    var topLine = new Rectangle
                    {
                        Fill = brush,
                        Height = thickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    Grid.SetColumnSpan(topLine, Math.Max(1, colCount));
                    grid.Children.Add(topLine);

                    var bottomLine = new Rectangle
                    {
                        Fill = brush,
                        Height = thickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Bottom
                    };
                    Grid.SetColumnSpan(bottomLine, Math.Max(1, colCount));
                    grid.Children.Add(bottomLine);
                }
            }
            else
            {
                // Линии между строками + верхняя и нижняя границы
                int startIndex = showOutline ? 0 : 1;
                int endIndex = showOutline ? rowCount : rowCount - 1;

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var line = new Rectangle
                    {
                        Fill = brush,
                        Height = thickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = i == rowCount ? VerticalAlignment.Bottom : VerticalAlignment.Top
                    };

                    int row = i == rowCount ? rowCount - 1 : i;
                    Grid.SetRow(line, row);
                    Grid.SetColumnSpan(line, Math.Max(1, colCount));
                    grid.Children.Add(line);
                }
            }

            // Вертикальные линии
            if (colCount == 0)
            {
                // Только левая и правая границы (если showOutline = true)
                if (showOutline)
                {
                    var leftLine = new Rectangle
                    {
                        Fill = brush,
                        Width = thickness,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    Grid.SetRowSpan(leftLine, Math.Max(1, rowCount));
                    grid.Children.Add(leftLine);

                    var rightLine = new Rectangle
                    {
                        Fill = brush,
                        Width = thickness,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    Grid.SetRowSpan(rightLine, Math.Max(1, rowCount));
                    grid.Children.Add(rightLine);
                }
            }
            else
            {
                // Линии между столбцами + левая и правая границы
                int startIndex = showOutline ? 0 : 1;
                int endIndex = showOutline ? colCount : colCount - 1;

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var line = new Rectangle
                    {
                        Fill = brush,
                        Width = thickness,
                        HorizontalAlignment = i == colCount ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    int col = i == colCount ? colCount - 1 : i;
                    Grid.SetColumn(line, col);
                    Grid.SetRowSpan(line, Math.Max(1, rowCount));
                    grid.Children.Add(line);
                }
            }
        }
    }
}