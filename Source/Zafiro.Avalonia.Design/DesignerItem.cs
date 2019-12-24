using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Zafiro.Avalonia.Design
{
    public class DesignerItem : ContentControl
    {
        public DesignerItem()
        {
            IsSelectedProperty.Changed.Subscribe(x => ((DesignerItem)x.Sender).PseudoClasses.Set(":selected", (bool) x.NewValue));
            IsEditingProperty.Changed.Subscribe(x =>
            {
                IsEditingChanged((bool) x.NewValue);
            });
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
            var firstChild = this.GetVisualDescendants().OfType<IInputElement>().FirstOrDefault();
            FocusManager.Instance.Focus(firstChild);
        }

        private void SetEditState(bool isEditing)
        {
            PseudoClasses.Set(":editing", isEditing);
        }

        public static readonly AvaloniaProperty LeftProperty = AvaloniaProperty.Register<DesignerItem, double>(
            "Left", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty TopProperty = AvaloniaProperty.Register<DesignerItem, double>(
            "Top", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty IsSelectedProperty = AvaloniaProperty.Register<DesignerItem, bool>(
            "IsSelected", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty IsEditingProperty = AvaloniaProperty.Register<DesignerItem, bool>(
            "IsEditing", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty AngleProperty = AvaloniaProperty.Register<DesignerItem, double>(
            "Angle", default, false, BindingMode.TwoWay);

        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private Control mover;
        private CompositeDisposable moverSubscriptions = new CompositeDisposable();

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public double Left
        {
            get => (double) GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public double Top
        {
            get => (double) GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            Mover = e.NameScope.Find<Control>("Mover");

            base.OnTemplateApplied(e);
        }

        public Control Mover
        {
            get => mover;
            set
            {
                moverSubscriptions?.Dispose();
                moverSubscriptions = new CompositeDisposable();

                mover = value;

                if (mover != null)
                {
                    Observable
                        .FromEventPattern<EventHandler<RoutedEventArgs>, RoutedEventArgs>(
                            h => mover.Tapped += h,
                            h => mover.Tapped -= h)
                        .Do(x => x.EventArgs.Handled = true)
                        .Select(x => Unit.Default)
                        .Subscribe(SelectionRequest)
                        .DisposeWith(moverSubscriptions);

                    Observable
                        .FromEventPattern<EventHandler<RoutedEventArgs>, RoutedEventArgs>(
                            h => mover.DoubleTapped += h,
                            h => mover.DoubleTapped -= h)
                        .Do(x => x.EventArgs.Handled = true)
                        .Select(x => Unit.Default)
                        .Subscribe(EditRequest)
                        .DisposeWith(moverSubscriptions);
                }
            }
        }

        public ISubject<Unit> EditRequest { get; set; } = new Subject<Unit>();

        public ISubject<Unit> SelectionRequest { get;  } = new Subject<Unit>();
    }
}