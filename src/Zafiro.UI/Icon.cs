using ReactiveUI.SourceGenerators;

namespace Zafiro.UI;

public interface IIcon
{
    string? Source { get; }
}

public partial class Icon : ReactiveObject, IIcon
{
    [Reactive] private string? source;

    public Icon()
    {
    }
    
    public Icon(string? source)
    {
        this.source = source;
    }
}

public partial class BigIcon : ReactiveObject, IIcon
{
    [Reactive] private string? source;

    public BigIcon()
    {
        
    }
    
    public BigIcon(string? source)
    {
        this.source = source;
    }
}