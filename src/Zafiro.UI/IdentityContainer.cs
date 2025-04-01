using ReactiveUI.SourceGenerators;

namespace Zafiro.UI;

public partial class IdentityContainer<T> : ReactiveObject
{
    [Reactive]
    private T content = default!;
}