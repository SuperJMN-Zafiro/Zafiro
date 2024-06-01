using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class MaybeEx
{
    public static Maybe<T> FromNullableStruct<T>(T? value) where T : struct
    {
        return value.HasValue ? Maybe<T>.From(value.Value) : new Maybe<T>(); // Suponiendo que hay un constructor por defecto que maneje la ausencia de valor
    }
}