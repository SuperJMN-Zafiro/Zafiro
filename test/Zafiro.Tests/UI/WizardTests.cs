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
        var steps = WizardBuilder.StartWith(() => new MyPage(), page => page.DoSomething)
            .BuildSteps();
        //var zardo = new Wizardo(steps.ToList());
    }

    [Fact]
    public void Wizard_page_is_set_initially()
    {
        var steps = WizardBuilder.StartWith(() => new MyPage(), page => page.DoSomething)
            .BuildSteps();
        var zardo = new Wizardo(steps.ToList());
        Assert.IsType<MyPage>(zardo.CurrentPage);
    }
}

public partial class Wizardo : ReactiveObject
{
    [ObservableAsProperty] private object currentPage;
    [ObservableAsProperty] private IWizardStep currentStep;
    [Reactive] private int currentStepIndex;

    public Wizardo(IList<IWizardStep> steps)
    {
        currentStepHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStepIndex)
            .Select(index => steps[index])
            .ToProperty(this, wizardo => wizardo.CurrentStep);

        currentPageHelper = this.WhenAnyValue(wizardo => wizardo.CurrentStep)
            .Select(step =>
            {
                var value = step.CreatePage(null);
                return value;
            })
            .ToProperty(this, wizardo => wizardo.CurrentPage);
    }
}

public class MyPage : ReactiveObject
{
    public MyPage()
    {
        DoSomething = EnhancedCommand.Create(ReactiveCommand.Create(() => Result.Success(1234)));
    }

    public EnhancedCommand<Result<int>> DoSomething { get; }
}