using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
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
    public void Page_is_set()
    {
        var steps = WizardBuilder.StartWith(() => new MyPage(), page => page.DoSomething)
            .BuildSteps();
        var zardo = new Wizard(steps.ToList());

        Assert.NotNull(zardo.CurrentPage);
        Assert.NotNull(zardo.CurrentPage.NextCommand);
        Assert.NotNull(zardo.CurrentPage.Title);
        Assert.IsType<MyPage>(zardo.CurrentPage.Content);
    }

    [Fact]
    public void Page_go_next()
    {
        var steps = WizardBuilder
            .StartWith(() => new MyPage(), page => page.DoSomething)
            .Then(i => new MyIntPage(i), _ => ReactiveCommand.Create(() => Result.Success("")).Enhance())
            .BuildSteps();

        var zardo = new Wizard(steps.ToList());

        zardo.NextCommand.Execute().Subscribe();

        Assert.NotNull(zardo.CurrentPage);
        Assert.NotNull(zardo.CurrentPage.NextCommand);
        Assert.NotNull(zardo.CurrentPage.Title);
        Assert.IsType<MyIntPage>(zardo.CurrentPage.Content);
    }
}

public class MyIntPage
{
    public MyIntPage(int number)
    {
        Number = number;
    }

    public int Number { get; }
}

public partial class Wizard : ReactiveObject
{
    [ObservableAsProperty] private Page currentPage;
    [ObservableAsProperty] private IWizardStep currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> nextCommand;
    private Stack<object> previousValues = new();

    public Wizard(IList<IWizardStep> steps)
    {
        currentStepHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStepIndex)
            .Select(index => steps[index])
            .ToProperty(this, wizardo => wizardo.CurrentStep);

        currentPageHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.CreatePage(param);
                var value = new Page(page, step.GetNextCommand(page), "Title");
                return value;
            })
            .ToProperty(this, wizardo => wizardo.CurrentPage);

        nextCommandHelper = this.WhenAnyValue(wizard => wizard.CurrentPage.NextCommand!).ToProperty(this, x => x.NextCommand);

        NextCommand.Successes().Subscribe(value =>
        {
            previousValues.Push(value);
            CurrentStepIndex++;
        });
    }
}