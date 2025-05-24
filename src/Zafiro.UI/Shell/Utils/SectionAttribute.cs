namespace Zafiro.UI.Shell.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class SectionAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}