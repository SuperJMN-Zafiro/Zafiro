using ReactiveUI.SourceGenerators;

namespace Zafiro.UI.Navigation.Sections;

public abstract partial class Section : ReactiveObject, ISection
{
    public bool IsPrimary { get; set; } = true;
    [Reactive] private bool isVisible = true;
    [Reactive] private int sortOrder = 0;
}