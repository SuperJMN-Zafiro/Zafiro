using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Cimbalino.Toolkit.Extensions;
using Microsoft.Toolkit.Uwp.UI.Helpers;

namespace Zafiro.Uwp.Designer
{
    public class DesignerItem : ContentControl
    {
        public DesignerItem()
        {
            DefaultStyleKey = typeof(DesignerItem);
            Loaded += OnLoaded;    
            Unloaded += OnUnloaded;

            RegisterPropertyChangedCallback(WidthProperty, (sender, dp) => { UpdateCanResize(); });
            RegisterPropertyChangedCallback(HeightProperty, (sender, dp) => { UpdateCanResize(); });
            UpdateCanResize();
        }

        private void UpdateCanResize()
        {
            CanResize = !(double.IsNaN(Width) || double.IsNaN(Height));
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(DesignerItem), new PropertyMetadata(default(double)));

        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetSelectionState(false);
            SetEditState(false);
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left", typeof(double), typeof(DesignerItem), new PropertyMetadata(default(double)));

        public double Left
        {
            get { return (double) GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top", typeof(double), typeof(DesignerItem), new PropertyMetadata(default(double)));

  
        public double Top
        {
            get { return (double) GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(DesignerItem), new PropertyMetadata(default(bool), PropertyChangedCallback));

        private IObservable<EventPattern<TappedRoutedEventArgs>> onTapObs;

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs changedArgs)
        {
            var d = (DesignerItem) dependencyObject;
            d.OnSelected((bool)changedArgs.OldValue, (bool)changedArgs.NewValue);
        }

        private void OnSelected(bool oldValue, bool newValue)
        {
            SetSelectionState(newValue);
        }

        private void SetSelectionState(bool newValue)
        {
            VisualStateManager.GoToState(this, newValue ? "Selected" : "Unselected", true);
        }

        private void SetEditState(bool newValue)
        {
            VisualStateManager.GoToState(this, newValue ? "Editing" : "Default", true);
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public ISubject<EventPattern<TappedRoutedEventArgs>> SelectionRequest { get; } = new Subject<EventPattern<TappedRoutedEventArgs>>();
        public ISubject<Unit> EditRequest { get; } = new Subject<Unit>();


        protected override void OnApplyTemplate()
        {
            var mover = (FrameworkElement)GetTemplateChild("Mover");
            if (mover != null)
            {
                // TODO: those subscriptions should be disposed eventually!

                Observable
                    .FromEventPattern<TappedEventHandler, TappedRoutedEventArgs>(h => mover.Tapped += h, h => mover.Tapped -= h)
                    .Subscribe(SelectionRequest);

                Observable
                    .FromEventPattern<DoubleTappedEventHandler, DoubleTappedRoutedEventArgs>(h => mover.DoubleTapped += h,
                        h => mover.DoubleTapped -= h)
                    .Select(_ => Unit.Default)
                    .Subscribe(EditRequest);
            }

            base.OnApplyTemplate();
        }

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
            "IsEditing", typeof(bool), typeof(DesignerItem), new PropertyMetadata(default(bool), IsEditingChanged));

        private static void IsEditingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (DesignerItem) dependencyObject;
            var newValue = (bool) dependencyPropertyChangedEventArgs.NewValue;
            target.IsEditingChanged(newValue);
        }

        private void IsEditingChanged(bool newValue)
        {
            SetEditState(newValue);

            if (newValue)
            {
                TrySetFocus();
            }
        }

        private void TrySetFocus()
        {
            var child = this.GetVisualDescendents<Control>();
            child.FirstOrDefault()?.Focus(FocusState.Programmatic);
        }

        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public Rect Bounds => new Rect(Left, Top, ActualWidth, ActualHeight);

        public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register("CanResize", typeof(bool), typeof(DesignerItem), new PropertyMetadata(default(bool)));

        public bool CanResize
        {
            get { return (bool) GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }
    }
}