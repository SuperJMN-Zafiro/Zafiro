using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using ReactiveUI;

namespace Zafiro.Uwp.Design
{
    public sealed class DesignerSurface : ItemsControl
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(IEnumerable), typeof(DesignerSurface), new PropertyMetadata(default(IEnumerable)));

        public static readonly DependencyProperty IsMultiSelectionEnabledProperty = DependencyProperty.Register(
            "IsMultiSelectionEnabled", typeof(bool), typeof(DesignerSurface), new PropertyMetadata(false));

        public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register(
            "CanResize", typeof(bool), typeof(DesignerSurface), new PropertyMetadata(default(bool)));

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly ObjectCommands objectCommands;

        public DesignerSurface()
        {
            DefaultStyleKey = typeof(DesignerSurface);

            Observable
                .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => Unloaded += h, h => Unloaded -= h)
                .Subscribe(_ => disposables.Dispose())
                .DisposeWith(disposables);

            Observable
                .FromEventPattern<TappedEventHandler, TappedRoutedEventArgs>(h => Tapped += h, h => Tapped -= h)
                .Subscribe(_ => ResetAll())
                .DisposeWith(disposables);

            SelectBoundsCommand = new DelegateCommand(SelectItemsInBounds);
            SelectedItemsObservable = this.WhenAnyValue(x => x.SelectedItems);
            SelectedContainersChanged =
                SelectedItemsObservable.Select(x =>
                    x == null
                        ? Enumerable.Empty<DesignerItem>()
                        : x.Cast<object>().Select(o => (DesignerItem) ContainerFromItem(o)));

            objectCommands = new ObjectCommands(this);
        }

        public ReactiveCommand<Unit, Unit> AlignToTopCommand => objectCommands.AlignToTopCommand;
        public ReactiveCommand<Unit, Unit> AlignToRightCommand => objectCommands.AlignToRightCommand;
        public ReactiveCommand<Unit, Unit> AlignToLeftCommand => objectCommands.AlignToLeftCommand;
        public ReactiveCommand<Unit, Unit> AlignToBottomCommand => objectCommands.AlignToBottomCommand;

        public IObservable<IEnumerable<DesignerItem>> SelectedContainersChanged { get; }

        public IObservable<IEnumerable> SelectedItemsObservable { get; }

        private IEnumerable<DesignerItem> ContainersUnderSelection => Containers.Where(i =>
        {
            var intersect = RectHelper.Union(SelectionBounds, i.Bounds);
            return intersect.Equals(SelectionBounds);
        });

        public DelegateCommand SelectBoundsCommand { get; }

        public BindingBase LeftBinding { get; set; }
        public BindingBase TopBinding { get; set; }
        public BindingBase HeightBinding { get; set; }
        public BindingBase WidthBinding { get; set; }
        public BindingBase AngleBinding { get; set; }

        private IEnumerable<DesignerItem> Containers => Items.Select(o => (DesignerItem) ContainerFromItem(o));

        public IEnumerable SelectedItems
        {
            get => (IEnumerable) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public bool IsMultiSelectionEnabled
        {
            get => (bool) GetValue(IsMultiSelectionEnabledProperty);
            set => SetValue(IsMultiSelectionEnabledProperty, value);
        }

        public Rect SelectionBounds { get; set; }

        public bool CanResize
        {
            get => (bool) GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }

        private void SelectItemsInBounds()
        {
            ClearSelection();

            foreach (var container in ContainersUnderSelection)
            {
                container.IsSelected = true;
            }

            SelectedItems = GetSelectedItems();
        }

        private void ResetAll()
        {
            ClearSelection();
            ClearEditMode();
        }

        private void ClearEditMode()
        {
            foreach (var designerItem in Containers)
            {
                designerItem.IsEditing = false;
            }
        }

        private void ClearSelection()
        {
            if (SelectedItems == null)
            {
                return;
            }

            foreach (var selectedItem in SelectedItems)
            {
                var di = (DesignerItem) ContainerFromItem(selectedItem);
                di.IsSelected = false;
            }

            SelectedItems = GetSelectedItems();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DesignerItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DesignerItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var di = (DesignerItem) element;

            SetBindings(di);

            di.SelectionRequest
                .Subscribe(ea => { OnSelectionRequest(di, ea.EventArgs); })
                .DisposeWith(disposables);

            di.EditRequest
                .Subscribe(_ => di.IsEditing = true)
                .DisposeWith(disposables);

            base.PrepareContainerForItemOverride(element, item);
        }

        private void OnSelectionRequest(DesignerItem di, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            tappedRoutedEventArgs.Handled = true;
            if (!IsMultiSelectionEnabled)
            {
                ClearSelection();
            }

            di.IsSelected = true;
            SelectedItems = GetSelectedItems();
        }

        private void SetBindings(DesignerItem di)
        {
            if (LeftBinding != null)
            {
                di.SetBinding(DesignerItem.LeftProperty, LeftBinding);
                di.SetBinding(Canvas.LeftProperty, new Binding { Path = new PropertyPath("Left"), Source = di });
            }

            if (TopBinding != null)
            {
                di.SetBinding(DesignerItem.TopProperty, TopBinding);
                di.SetBinding(Canvas.TopProperty, new Binding { Path = new PropertyPath("Top"), Source = di });
            }

            if (WidthBinding != null)
            {
                di.SetBinding(WidthProperty, WidthBinding);
            }

            if (HeightBinding != null)
            {
                di.SetBinding(HeightProperty, HeightBinding);
            }

            if (AngleBinding != null)
            {
                di.SetBinding(DesignerItem.AngleProperty, AngleBinding);
            }
        }

        private IList<object> GetSelectedItems()
        {
            return Containers.Where(x => x.IsSelected).Select(ItemFromContainer).ToList();
        }
    }
}