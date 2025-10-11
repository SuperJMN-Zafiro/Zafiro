using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using FluentAssertions;
using ReactiveUI;
using Zafiro.UI.Commands;

namespace Zafiro.Tests.UI;

public class CommandTests
{
    [Fact]
    public void Enhance_ReactiveCommand_SetsTextAndName()
    {
        var rc = ReactiveCommand.Create(() => Unit.Default);
        var cmd = rc.Enhance(text: "Back", name: "GoBack");

        cmd.Text.Should().Be("Back");
        cmd.Name.Should().Be("GoBack");
    }

    [Fact]
    public void Enhance_OnEnhancedCommand_ReturnsSameInstance_WhenNoChanges()
    {
        var baseCmd = ReactiveCommand.Create(() => 42).Enhance();
        var enhanced = baseCmd.Enhance();

        enhanced.Should().BeSameAs(baseCmd);
    }

    [Fact]
    public async Task Enhance_Maps_ResultUnit_To_Result_Success()
    {
        var rc = ReactiveCommand.CreateFromTask(async () => Result.Success(Unit.Default));
        var cmd = rc.Enhance();

        var result = await cmd.Execute().FirstAsync();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Enhance_Maps_ResultUnit_To_Result_Failure()
    {
        var rc = ReactiveCommand.CreateFromTask(async () => Result.Failure<Unit>("err"));
        var cmd = rc.Enhance();

        var result = await cmd.Execute().FirstAsync();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("err");
    }

    [Fact]
    public void Enhance_IEnhancedCommand_With_CanExecute_UsesNewGating()
    {
        var originalCanExecute = new BehaviorSubject<bool>(true);
        var rc = ReactiveCommand.Create(() => 1, originalCanExecute);
        var baseCmd = rc.Enhance();

        var newGate = new BehaviorSubject<bool>(false);
        var rewrapped = baseCmd.Enhance(canExecute: newGate);

        ((ICommand)rewrapped).CanExecute(null).Should().BeFalse();
        newGate.OnNext(true);
        ((ICommand)rewrapped).CanExecute(null).Should().BeTrue();
    }
}
