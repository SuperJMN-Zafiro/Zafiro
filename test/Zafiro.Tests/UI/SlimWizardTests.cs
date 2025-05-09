using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace Zafiro.Tests.UI;

public class SlimWizardTests
{
    [Fact]
    public void Build_steps()
    {
        WizardBuilder.StartWith(() => new MyPage(), page => page.DoSomething).BuildSteps();
    }

    [Fact]
    public void Page_is_set_after_build()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Build();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyPage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Go_next_sets_correct_page()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        wizard.Next.TryExecute();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyIntPage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Finished_wizard_should_stay_on_final_page_on_multiple_next()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        // Tries to go next, but nothing should happen
        wizard.Next.TryExecute();
        wizard.Next.TryExecute();
        wizard.Next.TryExecute();
        wizard.Next.TryExecute();

        Assert.Equal(wizard.CurrentStepIndex, 1);
    }

    [Fact]
    public void Finished_wizard_should_notify_result()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("Finished!")).Enhance())
            .Build();

        var result = "";
        wizard.Finished.Subscribe(value => result = value);
        wizard.Next.TryExecute();
        wizard.Next.TryExecute();

        Assert.Equal("Finished!", result);
    }

    [Fact]
    public void Finished_wizard_cannot_go_next()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        wizard.Next.TryExecute();
        wizard.Next.TryExecute();

        Assert.Equal(1, wizard.CurrentStepIndex);
    }

    [Fact]
    public void Initial_page_cannot_go_back()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        Observable.Return(Unit.Default).InvokeCommand(wizard.Back);

        Assert.Equal(wizard.CurrentStepIndex, 0);
    }

    [Fact]
    public void Page_failure_cannot_go_next()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), _ => ReactiveCommand.Create(() => Result.Failure<int>("Error")).Enhance())
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Failure<string>("Error")).Enhance())
            .Build();

        wizard.Next.TryExecute();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.Equal(wizard.CurrentStepIndex, 0);
        Assert.IsType<MyPage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Page_go_next_and_back()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        wizard.Next.TryExecute();
        wizard.Back.TryExecute();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyPage>(wizard.CurrentPage.Content);
    }
}

public static class ReactiveCommandExtensions
{
    public static IDisposable TryExecute(
        this IEnhancedCommand command)
    {
        return Observable.Return(Unit.Default).InvokeCommand(command);
    }
}