using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace Zafiro.Avalonia.Design
{
    public class DeltaContentControl : ContentControl
    {
        public static readonly AvaloniaProperty AngleProperty = AvaloniaProperty.Register<DeltaContentControl, double>(
            "Angle", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty AllowVerticalProperty =
            AvaloniaProperty.Register<DeltaContentControl, bool>(
                "AllowVertical", true, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty AllowHorizontalProperty =
            AvaloniaProperty.Register<DeltaContentControl, bool>(
                "AllowHorizontal", true, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty HorizontalProperty =
            AvaloniaProperty.Register<DeltaContentControl, double>(
                "Horizontal", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty VerticalProperty =
            AvaloniaProperty.Register<DeltaContentControl, double>(
                "Vertical", default, false, BindingMode.TwoWay);

        private MyThumb thumb;

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

        public double Angle
        {
            get => (double) GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public MyThumb Thumb
        {
            get => thumb;
            private set
            {
                thumb = value;
                if (thumb != null)
                {
                    thumb.DragDelta += ThumbOnDragDelta;
                }
            }
        }

        private void ThumbOnDragDelta(object sender, VectorEventArgs e)
        {
            var vector = e.Vector;
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

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            Thumb = e.NameScope.Find<MyThumb>("Thumb");

            base.OnTemplateApplied(e);
        }

        private static Vector ProjectVector(double angle, Vector vector)
        {
            var angleRad = angle * Math.PI * 2 / 360;
            var x = new Vector((float) Math.Cos(angleRad), -(float) Math.Sin(angleRad));
            var y = new Vector((float) Math.Sin(angleRad), (float) Math.Cos(angleRad));

            var xProj = Vector.Dot(vector, x);
            var yProj = Vector.Dot(vector, y);

            var result = new Vector(xProj, yProj);
            return result;
        }
    }
}