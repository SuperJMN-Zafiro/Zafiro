namespace Zafiro.UI;

public class PopupModel
{
    public PopupModel(object content, string title, IEnumerable<Option> options)
    {
        Title = title;
        Content = content;
        Options = options;
    }

    public string Title { get; }

    public IEnumerable<Option> Options { get; set; }

    public object Content { get; }
}