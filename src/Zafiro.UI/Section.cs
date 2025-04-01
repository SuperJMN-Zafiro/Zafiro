using CSharpFunctionalExtensions;

namespace Zafiro.UI;

public class Section : ISection
{
    public Section(string title, object content, object? icon)
    {
        Title = title;
        Content = content;
        Icon = icon;
    }

    public string Title { get; }
    public object Icon { get; }
    public object Content { get; }
}