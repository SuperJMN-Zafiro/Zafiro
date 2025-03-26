namespace Zafiro.DivineBytes;

public static class NamedWithPathMixin
{
    public static Path FullPath(this INamedWithPath rooted) => rooted.Path.Combine(rooted.Name);
}