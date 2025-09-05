namespace Zafiro.UI.Shell.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class SectionAttribute(string? name = null, string? icon = null, int sortIndex = 0, Type? contractType = null) : Attribute
{
    public string? Name { get; } = name;
    public string? Icon { get; } = icon;
    public int SortIndex { get; } = sortIndex;

    /// <summary>
    /// Optional service contract to resolve as the initial content for this section.
    /// If null, the annotated class type will be used.
    /// </summary>
    public Type? ContractType { get; } = contractType;
}