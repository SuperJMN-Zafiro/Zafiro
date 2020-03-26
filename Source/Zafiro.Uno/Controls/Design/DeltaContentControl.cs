using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Zafiro.Uno.Controls.Design
{
    public partial class DeltaContentControl : ContentControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty HorizontalProperty = DependencyProperty.Register(
            "Horizontal", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty VerticalProperty = DependencyProperty.Register(
            "Vertical", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty AllowVerticalProperty = DependencyProperty.Register(
            "AllowVertical", typeof(bool), typeof(DeltaContentControl), new PropertyMetadata(true));

        public static readonly DependencyProperty AllowHorizontalProperty = DependencyProperty.Register(
            "AllowHorizontal", typeof(bool), typeof(DeltaContentControl), new PropertyMetadata(true));

        public static readonly DependencyProperty SharedHorizontalProperty = DependencyProperty.Register(
            "SharedHorizontal", typeof(double), typeof(DeltaContentControl), new PropertyMetadata(double.NaN));

        private Thumb thumb;

        public DeltaContentControl()
        {
            DefaultStyleKey = typeof(DeltaContentControl);
        }

        public Thumb Thumb
        {
            get => thumb;
            private set
            {
                thumb = value;
                thumb.DragDelta += ThumbOnDragDelta;
                thumb.Tapped += ThumbOnTapped;
            }
        }

        public double Angle
        {
            get => (double) GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public double Horizontal
        {
            get => (double) GetValue(HorizontalProperty);
            set => SetValue(HorizontalProperty, value);
        }

        public double Vertical
        {
            get => (double) GetValue(VerticalProperty);
            set => SetValue(VerticalProperty, value);
        }

        public bool AllowVertical
        {
            get => (bool) GetValue(AllowVerticalProperty);
            set => SetValue(AllowVerticalProperty, value);
        }

        public bool AllowHorizontal
        {
            get => (bool) GetValue(AllowHorizontalProperty);
            set => SetValue(AllowHorizontalProperty, value);
        }

        private void ThumbOnTapped(object sender, TappedRoutedEventArgs e)
        {
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
            Thumb = (Thumb) GetTemplateChild("Thumb");
            base.OnApplyTemplate();
        }
    }
}