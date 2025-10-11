using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;

namespace Zafiro.Tests.UI;

public class MyPage : ReactiveObject
{
    public MyPage()
    {
        DoSomething = ReactiveCommand.Create(() => Result.Success(1234)).Enhance();
    }

    public IEnhancedCommand<Result<int>> DoSomething { get; }
}