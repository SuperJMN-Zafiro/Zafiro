namespace Zafiro.UI.Navigation.Sections;

public interface IContentSection : INamedSection
{
    IObservable<object> Content { get; }
    Type RootType { get; }
}