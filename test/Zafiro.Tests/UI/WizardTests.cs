using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
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
}

public partial class Wizard : ReactiveObject
{
    [ObservableAsProperty] private Page currentPage;
    [ObservableAsProperty] private IWizardStep currentStep;
    [Reactive] private int currentStepIndex;

    public Wizard(IList<IWizardStep> steps)
    {
        currentStepHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStepIndex)
            .Select(index => steps[index])
            .ToProperty(this, wizardo => wizardo.CurrentStep);

        currentPageHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStep)
            .Select(step =>
            {
                var page = step.CreatePage(null);
                var value = new Page(page, step.GetNextCommand(page), "Title");
                return value;
            })
            .ToProperty(this, wizardo => wizardo.CurrentPage);
    }
}

public record Page(object Content, IEnhancedCommand<Result<object>>? NextCommand, string Title);

public class MyPage : ReactiveObject
{
    public MyPage()
    {
        DoSomething = EnhancedCommand.Create(ReactiveCommand.Create(() => Result.Success(1234)));
    }

    public EnhancedCommand<Result<int>> DoSomething { get; }
}