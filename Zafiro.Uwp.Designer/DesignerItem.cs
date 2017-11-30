using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Zafiro.Uwp.Designer
{
    public class DesignerItem : ContentControl
    {
        public DesignerItem()
        {
            DefaultStyleKey = typeof(DesignerItem);
            Loaded += OnLoaded;            
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
            var e = (FrameworkElement)GetTemplateChild("Mover");

            // TODO: those subscriptions should be disposed eventually!

            Observable
                .FromEventPattern<TappedEventHandler, TappedRoutedEventArgs>(h => e.Tapped += h, h => e.Tapped -= h)
                .Subscribe(SelectionRequest);
            
            Observable
                .FromEventPattern<DoubleTappedEventHandler, DoubleTappedRoutedEventArgs>(h => e.DoubleTapped += h,
                    h => e.DoubleTapped -= h)
                .Select(_ => Unit.Default)
                .Subscribe(EditRequest);

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
        }

        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }
    }
}