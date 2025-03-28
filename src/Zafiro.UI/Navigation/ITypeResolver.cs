using System.Diagnostics.CodeAnalysis;

namespace Zafiro.UI.Navigation;

public interface ITypeResolver
{
    [return: NotNull]
    T Resolve<T>();
}