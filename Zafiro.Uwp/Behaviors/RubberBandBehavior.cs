using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace Zafiro.Uwp.Controls.Behaviors
{
    public class RubberBandBehavior : Behavior<FrameworkElement>
    {
        private IDisposable subscription;

        private Point Origin { get; set; }

        private Popup Popup { get; set; }

        public DataTemplate RubberBandTemplate { get; set; }

        protected override void OnAttached()
        {
            var target = AssociatedObject;
            var refe = Window.Current.Content;

            var downs = Observable
                .FromEventPattern<PointerEventHandler, PointerRoutedEventArgs>(x => target.PointerPressed += x,
                    x => target.PointerPressed -= x)
                .Select(e => e.EventArgs.GetCurrentPoint(refe).Position)
                .Where(p => Distance(Origin, p) > 5);

            var moves = Observable
                .FromEventPattern<PointerEventHandler, PointerRoutedEventArgs>(x => target.PointerMoved += x,
                    x => target.PointerMoved -= x)
                .Select(e => e.EventArgs.GetCurrentPoint(refe).Position);

            var ups = Observable
                .FromEventPattern<PointerEventHandler, PointerRoutedEventArgs>(x => target.PointerReleased += x,
                    x => target.PointerReleased -= x)
                .Select(e => e.EventArgs.GetCurrentPoint(refe).Position)
                .Where(_ => Popup != null);

            subscription = moves
                .SkipUntil(downs.Do(OnPress))
                .TakeUntil(ups.Do(OnRelease))
                .Repeat()
                .Subscribe(OnMove);
        }

        private static double Distance(Point a, Point b)
        {
            var x1 = a.X;
            var x2 = b.X;

            var y1 = a.Y;
            var y2 = b.Y;

            return Distance(x1, y1, x2, y2);
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private void OnRelease(Point p)
        {
            RemovePopup();
            var transform = ((UIElement)Window.Current.Content).TransformToVisual(AssociatedObject);
            LastRectBounds = new Rect(transform.TransformPoint(Origin), transform.TransformPoint(p));
            TryExecuteCommand();
        }

        private void TryExecuteCommand()
        {
            if (Command == null)
            {
                return;
            }

            if (Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(RubberBandBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(RubberBandBehavior), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object) GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty LastRectBoundsProperty = DependencyProperty.Register(
            "LastRectBounds", typeof(Rect), typeof(RubberBandBehavior), new PropertyMetadata(default(Rect)));

        public Rect LastRectBounds
        {
            get { return (Rect) GetValue(LastRectBoundsProperty); }
            set { SetValue(LastRectBoundsProperty, value); }
        }

        private void RemovePopup()
        {
            if (Popup == null)
            {
                return;
            }

            Popup.IsOpen = false;
            Popup = null;
        }

        private void OnMove(Point p)
        {
            UpdatePopupBounds(Popup, new Rect(Origin, p));
        }

        private void OnPress(Point point)
        {
            var child = CreateChild();
            Popup = new Popup
            {
                IsOpen = true,
                IsHitTestVisible = false,
                Child = child
            };
            Origin = point;
            UpdatePopupBounds(Popup, new Rect(Origin, Origin));
        }

        private FrameworkElement CreateChild()
        {
            return RubberBandTemplate == null
                ? new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(80, 173, 216, 230)),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue)
                }
                : (FrameworkElement)RubberBandTemplate.LoadContent();
        }

        private static void UpdatePopupBounds(Popup p, Rect rect)
        {
            p.HorizontalOffset = rect.X;
            p.VerticalOffset = rect.Y;

            var child = (FrameworkElement)p.Child;

            child.Width = rect.Width;
            child.Height = rect.Height;
        }

        protected override void OnDetaching()
        {
            subscription?.Dispose();
        }
    }
}