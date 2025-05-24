using ReactiveUI.SourceGenerators;

namespace Zafiro.Avalonia;

public partial class Icon : ReactiveObject
{
    [Reactive] private string iconId;
}