using System.Windows.Input;
using CSharpFunctionalExtensions;

namespace Zafiro.UI.Navigation.Sections;

public class ContentSection<T>(string name, Func<T> getViewModel, object? icon) : Section, IContentSection
{
    public string Name { get; } = name;

    Func<object?> IContentSection.GetViewModel => () => GetViewModel();
    public Func<T> GetViewModel { get; } = getViewModel;

    public object? Icon { get; } = icon;

    public object? Content => GetViewModel();
}