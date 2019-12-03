using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using ReactiveUI;
using Zafiro.Core.ObjectEditor;
using Zafiro.Core.Values;

namespace Zafiro.Uwp.ObjectEditor
{
    public class PropertyItem : PropertyItem<FrameworkElement>
    {
        private readonly GroupSetter groupSetter;
        private readonly GroupGetter groupGetter;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public FrameworkElement ValueEditor { get; }
        
        public PropertyItem(FrameworkElement valueEditor, PropertyInfo propertyInfo, IList<object> targets) : base(
            valueEditor, propertyInfo, targets)
        {
            ValueEditor = valueEditor;
            groupSetter = new GroupSetter(propertyInfo);
            groupGetter = new GroupGetter(propertyInfo);
            SubscribeToPropertyChangesOf(targets.OfType<INotifyPropertyChanged>());
        }

        private void SubscribeToPropertyChangesOf(IEnumerable<INotifyPropertyChanged> observables)
        {
            var subscriptions = from observable in observables
                let subscription =
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            h => observable.PropertyChanged += h, h => PropertyChanged -= h)
                        .Subscribe(args => ObsOnPropertyChanged(args.EventArgs.PropertyName))
                select subscription;

            foreach (var subscription in subscriptions)
            {
                disposables.Add(subscription);
            }
        }

        private void ObsOnPropertyChanged(string propertyName)
        {
            if ((string) PropertyName == propertyName)
            {
                this.RaisePropertyChanged(nameof(Value));
            }
        }

        public object Value
        {
            get => groupGetter.GetValue(Targets);
            set
            {
                groupSetter.Set(Targets, value);
                this.RaisePropertyChanged();
            }
        }

        public override void Dispose()
        {
            disposables.Dispose();
        }
    }
}