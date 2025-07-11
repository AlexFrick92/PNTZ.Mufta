using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;


namespace PNTZ.Mufta.TPCApp.View
{
    public class WatermarkAdorner : Adorner
    {
        private readonly TextBlock _textBlock;

        public WatermarkAdorner(UIElement adornedElement, string watermark) : base(adornedElement)
        {
            IsHitTestVisible = false;

            _textBlock = new TextBlock
            {
                Text = watermark,
                Foreground = Brushes.Gray,
                Margin = new Thickness(2, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontStyle = FontStyles.Italic,
                Opacity = 0.5,
            };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            _textBlock.Measure(adornedElementRect.Size);
            _textBlock.Arrange(adornedElementRect);

            var padding = new Thickness(0);
            if (AdornedElement is System.Windows.Controls.Control control)
                padding = control.Padding;

            var point = new Point(
                padding.Left + 4,
                (adornedElementRect.Height - _textBlock.DesiredSize.Height) / 2);

            drawingContext.DrawText(
                new FormattedText(
                    _textBlock.Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(_textBlock.FontFamily, _textBlock.FontStyle, _textBlock.FontWeight, _textBlock.FontStretch),
                    _textBlock.FontSize,
                    _textBlock.Foreground),
                point);
        }
    }
}
