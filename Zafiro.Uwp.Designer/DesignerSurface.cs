using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;


namespace Zafiro.Uwp.Designer
{
    public sealed class DesignerSurface : ItemsControl
    {
        private CompositeDisposable tapDisposables;

        public DesignerSurface()
        {
            DefaultStyleKey = typeof(DesignerSurface);

            tapDisposables = new CompositeDisposable();

            Observable
                .FromEventPattern<TappedEventHandler, TappedRoutedEventArgs>(h => Tapped += h, h => Tapped -= h)
                .Subscribe(_ => ResetAll());

            SelectBoundsCommand = new DelegateCommand(SelectItemsInBounds);
        }

        private void SelectItemsInBounds()
        {
            ClearSelection();

            foreach (var ci in ContainersUndersSelection)
            {
                ci.IsSelected = true;
            }                  

            SelectedItems = GetSelectedItems();
        }

        private IEnumerable<DesignerItem> ContainersUndersSelection => Containers.Where(i =>
        {
            var intersect = RectHelper.Union(SelectionBounds, i.Bounds);
            return intersect.Equals(SelectionBounds);
        });

        public DelegateCommand SelectBoundsCommand { get; }

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
                var di = (DesignerItem)ContainerFromItem(selectedItem);
                di.IsSelected = false;
            }

            SelectedItems = GetSelectedItems();
        }

        public BindingBase LeftBinding { get; set; }
        public BindingBase TopBinding { get; set; }
        public BindingBase HeightBinding { get; set; }
        public BindingBase WidthBinding { get; set; }
        public BindingBase AngleBinding { get; set; }

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

            var subscriptions = new CompositeDisposable();

            subscriptions.Add(di.SelectionRequest.Subscribe(ea =>
            {
                ea.EventArgs.Handled = true;
                if (IsMultiSelectionEnabled)
                {
                    ClearSelection();
                }
                di.IsSelected = true;
                SelectedItems = GetSelectedItems();
            }));

            subscriptions.Add(di.EditRequest.Subscribe(_ => di.IsEditing = true));

            base.PrepareContainerForItemOverride(element, item);
        }

        private void SetBindings(DesignerItem di)
        {
            if (LeftBinding != null)
            {
                di.SetBinding(DesignerItem.LeftProperty, LeftBinding);
            }

            if (TopBinding != null)
            {
                di.SetBinding(DesignerItem.TopProperty, TopBinding);
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

        private IEnumerable<DesignerItem> Containers => Items.Select(o => (DesignerItem)ContainerFromItem(o));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(ObservableCollection<object>), typeof(DesignerSurface), new PropertyMetadata(default(IEnumerable)));

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable) GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty IsMultiSelectionEnabledProperty = DependencyProperty.Register(
            "IsMultiSelectionEnabled", typeof(bool), typeof(DesignerSurface), new PropertyMetadata(true));

        public bool IsMultiSelectionEnabled
        {
            get { return (bool) GetValue(IsMultiSelectionEnabledProperty); }
            set { SetValue(IsMultiSelectionEnabledProperty, value); }
        }

        public Rect SelectionBounds { get;set; }

        public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register(
            "CanResize", typeof(bool), typeof(DesignerSurface), new PropertyMetadata(default(bool)));

        public bool CanResize
        {
            get { return (bool) GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }
    }
}
