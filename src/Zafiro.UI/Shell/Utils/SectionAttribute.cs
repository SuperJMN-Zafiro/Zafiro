namespace Zafiro.UI.Shell.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class SectionAttribute(string? icon = null, int sortIndex = 0) : Attribute
{
    public string? Icon { get; } = icon;
    public int SortIndex { get; } = sortIndex;
}