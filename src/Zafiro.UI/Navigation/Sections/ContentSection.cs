using System.Reactive.Linq;

namespace Zafiro.UI.Navigation.Sections;

public class ContentSection<T>(string name, IObservable<T> content, object? icon) : Section, IContentSection where T : class
{
    public string Name { get; } = name;

    public object? Icon { get; } = icon;

    public Type RootType { get; } = typeof(T);

    public IObservable<object> Content => content.Select(object (arg) => arg);
}