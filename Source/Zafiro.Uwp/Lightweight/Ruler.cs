using System.Globalization;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Zafiro.Uwp.Controls.Lightweight
{
    public class Ruler : Control
    {
        public Ruler()
        {
            DefaultStyleKey = typeof(Ruler);
        }

        protected override void OnApplyTemplate()
        {
            var canvas = (CanvasControl)GetTemplateChild("Canvas");
            canvas.Draw += CanvasOnDraw;
            base.OnApplyTemplate();
        }

        private void CanvasOnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.Antialiasing = CanvasAntialiasing.Aliased;
            
            double step = 24;
            int biggerTickStep = 4;
            var n = 0;
            for (double i = 0; i < this.ActualWidth; i += step)
            {
                if (n % biggerTickStep == 0)
                {
                    DrawBigTick(args, i);
                }
                else
                {
                    DrawSmallTick(args, i);
                }
                n++;
            }
        }

        private void DrawNumber(CanvasDrawEventArgs args, double x, double number)
        {
            var xfloat = (float)x;
            var start = new Vector2(xfloat, (float)(TickHeight / 2));
            var end = new Vector2(xfloat, (float)TickHeight);

            args.DrawingSession.DrawText(number.ToString(CultureInfo.InvariantCulture), start, Color.FromArgb(255,0,0,0));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            return new Size(size.Width, TickHeight);
        }


        private void DrawSmallTick(CanvasDrawEventArgs args, double x)
        {
            var xfloat = (float)x;
            var start = new Vector2(xfloat, (float) (TickHeight / 2));
            var end = new Vector2(xfloat, (float)TickHeight);
            args.DrawingSession.DrawLine(start, end, Color.FromArgb(200, 0, 0, 0), 1);
        }

        private void DrawBigTick(CanvasDrawEventArgs args, double x)
        {
            var xfloat = (float)x;
            var start = new Vector2(xfloat, 0);
            var end = new Vector2(xfloat, (float)TickHeight);
            args.DrawingSession.DrawLine(start, end, Color.FromArgb(255, 0, 0, 0), 1);
        }

        public static readonly DependencyProperty TickHeightProperty = DependencyProperty.Register(
            "TickHeight", typeof(double), typeof(Ruler), new PropertyMetadata(10D));

        public double TickHeight
        {
            get { return (double)GetValue(TickHeightProperty); }
            set { SetValue(TickHeightProperty, value); }
        }
    }
}