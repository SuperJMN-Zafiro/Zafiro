using System;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards;

namespace Zafiro.Tests.UI;

public class WizardTests
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
        Assert.NotNull(wizard.CurrentPage.NextCommand);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyPage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Page_go_next()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .Build();

        wizard.NextCommand.Execute().Subscribe();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.NextCommand);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyIntPage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Page_failure_cannot_go_next()
    {
        var wizard = WizardBuilder
            .StartWith(() => new MyPage(), _ => ReactiveCommand.Create(() => Result.Failure<int>("Error")).Enhance())
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Failure<string>("Error")).Enhance())
            .Build();

        wizard.NextCommand.Execute().Subscribe();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.NextCommand);
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

        wizard.NextCommand.Execute().Subscribe();
        wizard.BackCommand.Execute().Subscribe();

        Assert.NotNull(wizard.CurrentPage);
        Assert.NotNull(wizard.CurrentPage.NextCommand);
        Assert.NotNull(wizard.CurrentPage.Title);
        Assert.IsType<MyPage>(wizard.CurrentPage.Content);
    }
}