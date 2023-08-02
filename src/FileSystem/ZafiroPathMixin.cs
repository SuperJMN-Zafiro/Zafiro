using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public static class ZafiroPathMixin
{
    public static string NameWithoutExtension(this ZafiroPath path)
    {
        var last = path.RouteFragments.Last();
        var lastIndex = last.LastIndexOf('.');

        return lastIndex < 0 ? last : last[..lastIndex];
    }

    public static Maybe<string> Extension(this ZafiroPath path)
    {
        var last = path.RouteFragments.Last();
        var lastIndex = last.LastIndexOf('.');

        return lastIndex < 0 ? Maybe<string>.None : last[(lastIndex+1)..];
    }
}