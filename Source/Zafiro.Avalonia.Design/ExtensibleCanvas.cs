using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Zafiro.Avalonia.Design
{
    public class ExtensibleCanvas : Canvas
    {
        private readonly IDictionary<object, IDisposable> disposables = new Dictionary<object, IDisposable>();

        protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Subscribe(e.NewItems);
                    Unsubscribe(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Unsubscribe(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Subscribe(e.NewItems);
                    Unsubscribe(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Unsubscribe(Children);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.ChildrenChanged(sender, e);
        }

        private void Unsubscribe(IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                disposables.Remove(item);
            }
        }

        private void Subscribe(IEnumerable items)
        {
            var layoutables = items.OfType<Layoutable>();
            foreach (var layoutable in layoutables)
            {
                var layoutUpdated = Observable
                    .FromEventPattern<EventHandler, EventArgs>(
                        h => layoutable.LayoutUpdated += h,
                        h => layoutable.LayoutUpdated -= h)
                    .Select(_ => Unit.Default);

                var leftChangd = layoutable.GetObservable(LeftProperty).Select(_ => Unit.Default);
                var topChanged = layoutable.GetObservable(TopProperty).Select(_ => Unit.Default);

                disposables[layoutable] =
                    layoutUpdated
                        .Merge(leftChangd)
                        .Merge(topChanged)
                        .Throttle(TimeSpan.FromSeconds(1))
                        .ObserveOn(SynchronizationContext.Current)
                        .Subscribe(pattern => InvalidateMeasure());
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }
            
            var frameworkElements = Children.OfType<Control>().ToList();

            if (frameworkElements.Count == 0)
            {
                return new Size(0, 0);
            }

            var w = frameworkElements.Max(o => GetLeft(o) + o.DesiredSize.Width);
            var h = frameworkElements.Max(o => GetTop(o) + o.DesiredSize.Height);

            return new Size(w, h);
        }
    }
}