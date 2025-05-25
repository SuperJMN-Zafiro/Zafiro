using System.Reflection;

namespace Zafiro.UI.Shell.Utils;

public static class Extensions
{
    public static bool IsSection(this Type t)
    {
        return t is { IsClass: true, IsAbstract: false } &&
               t.Name.EndsWith("ViewModel") &&
               t.GetCustomAttribute<SectionAttribute>() != null;
    }
}