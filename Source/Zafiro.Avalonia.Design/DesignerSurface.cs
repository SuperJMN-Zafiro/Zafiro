using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Design
{
    public class DesignerSurface : ItemsControl
    {
        public static readonly AvaloniaProperty IsMultiSelectionEnabledProperty =
            AvaloniaProperty.Register<DesignerSurface, bool>(
                "IsMultiSelectionEnabled", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty SelectedItemsProperty =
            AvaloniaProperty.Register<DesignerSurface, IEnumerable>(
                "SelectedItems", default, false, BindingMode.TwoWay);

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public DesignerSurface()
        {
            Observable
                .FromEventPattern<EventHandler<RoutedEventArgs>, RoutedEventArgs>(
                    h => Tapped += h,
                    h => Tapped -= h)
                .Subscribe(s => ResetAll())
                .DisposeWith(disposables);
        }

        private void ResetAll()
        {
            ClearSelection();
            ClearEditMode();
            FocusManager.Instance.Focus(this);
        }

        private void ClearEditMode()
        {
            foreach (var designerItem in Containers)
            {
                designerItem.IsEditing = false;
            }
        }

        public Binding LeftBinding { get; set; }
        public Binding TopBinding { get; set; }
        public Binding WidthBinding { get; set; }
        public Binding HeightBinding { get; set; }
        public Binding AngleBinding { get; set; }

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

        private IEnumerable<DesignerItem> Containers =>
            ItemContainerGenerator.Containers.Select(x => x.ContainerControl).Cast<DesignerItem>();

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<DesignerItem>(this, ContentControl.ContentProperty,
                ContentControl.ContentTemplateProperty);
        }

        protected override void OnContainersMaterialized(ItemContainerEventArgs e)
        {
            foreach (var itemContainerInfo in e.Containers)
            {
                var di = (DesignerItem) itemContainerInfo.ContainerControl;
                di.SelectionRequest
                    .Subscribe(_ => OnSelectionRequest(di))
                    .DisposeWith(disposables);

                di.EditRequest
                    .Subscribe(_ => di.IsEditing = true)
                    .DisposeWith(disposables);

                SetBindings(di);
            }

            base.OnContainersMaterialized(e);
        }

        private void OnSelectionRequest(DesignerItem di)
        {
            if (!IsMultiSelectionEnabled)
            {
                ClearSelection();
            }

            di.IsSelected = true;
            SelectedItems = GetSelectedItems();
        }

        private IList<object> GetSelectedItems()
        {
            return ItemContainerGenerator.Containers
                .Where(x => x.ContainerControl is DesignerItem di && di.IsSelected)
                .Select(x => x.Item)
                .ToList();
        }


        private void ClearSelection()
        {
            if (SelectedItems != null)
            {
                foreach (var selectedItem in SelectedItems)
                {
                    var container = ItemContainerGenerator.Containers.First(x => x.Item == selectedItem);
                    var di = (DesignerItem) container.ContainerControl;
                    di.IsSelected = false;
                }
            }

            SelectedItems = GetSelectedItems();
        }

        private void SetBindings(DesignerItem containerControl)
        {
            containerControl.Bind(DesignerItem.LeftProperty, LeftBinding);
            containerControl.Bind(Canvas.LeftProperty, new Binding("Left") { Source = containerControl });

            containerControl.Bind(DesignerItem.TopProperty, TopBinding);
            containerControl.Bind(Canvas.TopProperty, new Binding("Top") {Source = containerControl});

            containerControl.Bind(WidthProperty, WidthBinding);

            containerControl.Bind(HeightProperty, HeightBinding);

            containerControl.Bind(DesignerItem.AngleProperty, AngleBinding);
        }
    }
}