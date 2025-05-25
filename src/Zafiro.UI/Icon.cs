using ReactiveUI.SourceGenerators;

namespace Zafiro.UI;

public partial class Icon : ReactiveObject
{
    [Reactive] private string? iconId;
}

public partial class BigIcon : ReactiveObject
{
    [Reactive] private string? iconId;
}