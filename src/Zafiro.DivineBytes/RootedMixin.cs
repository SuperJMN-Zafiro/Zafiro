namespace Zafiro.DivineBytes;

public static class RootedMixin
{
    public static ZafiroPath FullPath(this IRooted rooted) => rooted.Path.Combine(rooted.Name);
}