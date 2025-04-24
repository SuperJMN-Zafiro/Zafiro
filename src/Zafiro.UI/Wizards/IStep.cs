using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizards;

public interface IStep : IValidatable, IBusy
{
    bool AutoAdvance { get; }
    Maybe<string> Title => Maybe<string>.None;
}