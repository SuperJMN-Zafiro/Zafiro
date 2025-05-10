namespace Zafiro.UI.Wizards.Slim;

public interface IPage
{
    object Content { get; }
    string Title { get; }
    public int Index { get; }
}