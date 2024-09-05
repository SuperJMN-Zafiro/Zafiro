using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Custom;
using ReactiveUI;
using Zafiro.Progress;

namespace Sample;

public class ProgressBehavior : DisposingBehavior<ProgressBar>
{
    public static readonly StyledProperty<IProgress> ProgressProperty = AvaloniaProperty.Register<ProgressBehavior, IProgress>(
        nameof(Progress));

    public IProgress Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    protected override void OnAttached(CompositeDisposable disposables)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        this.WhenAnyValue(x => x.Progress)
            .Do(progress =>
            {
                if (progress is NotStarted)
                {
                    AssociatedObject.IsVisible = false;
                }
                else
                {
                    AssociatedObject.IsVisible = true;
                }
                
                if (progress is ProportionalProgress ratioProgress)
                {
                    AssociatedObject.IsIndeterminate = false;
                    AssociatedObject.Maximum = 1;
                    AssociatedObject.Value = ratioProgress.Ratio;
                }

                if (progress is Completed)
                {
                    AssociatedObject.IsIndeterminate = false;
                    AssociatedObject.Value = AssociatedObject.Maximum;
                }

                if (progress is Unknown)
                {
                    AssociatedObject.IsIndeterminate = true;
                }
            })
            .Subscribe()
            .DisposeWith(disposables);
    }
}