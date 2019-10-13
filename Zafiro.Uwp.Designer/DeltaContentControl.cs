using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Zafiro.Uwp.Designer
{
    public sealed class DeltaContentControl : ContentControl
    {
        private Thumb thumb;

        public DeltaContentControl()
        {
            this.DefaultStyleKey = typeof(DeltaContentControl);
        }

        public Thumb Thumb
        {
            get { return thumb; }
            private set
            {
                thumb = value;
                thumb.DragDelta += ThumbOnDragDelta;
            }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var vector = new Vector2((float) e.HorizontalChange, (float) e.VerticalChange);
            var result = ProjectVector(Angle, vector);

            var horzDelta = result.X;
            var vertDelta = result.Y;

            if (AllowHorizontal)
            {
                Horizontal += horzDelta;
            }

            if (AllowVertical)
            {
                Vertical += vertDelta;
            }
        }

        private static Vector2 ProjectVector(double angle, Vector2 vector)
        {
            var angleRad = angle * Math.PI * 2 / 360;
            var x = new Vector2((float) Math.Cos(angleRad), -(float) Math.Sin(angleRad));
            var y = new Vector2((float) Math.Sin(angleRad), (float) Math.Cos(angleRad));

            var xProj = Vector2.Dot(vector, x);
            var yProj = Vector2.Dot(vector, y);

            var result = new Vector2(xProj, yProj);
            return result;
        }

        protected override void OnApplyTemplate()
        {
            Thumb = (Thumb)GetTemplateChild("Thumb");
            base.OnApplyTemplate();
        }

        public static readonly DependencyProperty HorizontalProperty = DependencyProperty.Register(
            "Horizontal", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public double Horizontal
        {
            get { return (double)GetValue(HorizontalProperty); }
            set { SetValue(HorizontalProperty, value); }
        }

        public static readonly DependencyProperty VerticalProperty = DependencyProperty.Register(
            "Vertical", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public double Vertical
        {
            get { return (double)GetValue(VerticalProperty); }
            set { SetValue(VerticalProperty, value); }
        }

        public static readonly DependencyProperty AllowVerticalProperty = DependencyProperty.Register(
            "AllowVertical", typeof(bool), typeof(DeltaContentControl), new PropertyMetadata(true));

        public bool AllowVertical
        {
            get { return (bool)GetValue(AllowVerticalProperty); }
            set { SetValue(AllowVerticalProperty, value); }
        }

        public static readonly DependencyProperty AllowHorizontalProperty = DependencyProperty.Register(
            "AllowHorizontal", typeof(bool), typeof(DeltaContentControl), new PropertyMetadata(true));

        public bool AllowHorizontal
        {
            get { return (bool)GetValue(AllowHorizontalProperty); }
            set { SetValue(AllowHorizontalProperty, value); }
        }

        public static readonly DependencyProperty SharedHorizontalProperty = DependencyProperty.Register(
            "SharedHorizontal", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(double.NaN));
    }
}