using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.UI;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim.Builder;

namespace Zafiro.Tests.UI;

public class FluentWizardBuilderTests
{
    [Fact]
    public void Next_Always_builds_unconditional_step()
    {
        var wizard = WizardBuilder
            .StartWith(() => new SimplePage(), "First")
            .Next(page => page.Value).Always()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
        Assert.IsType<SimplePage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Next_WhenValid_builds_validatable_step()
    {
        var wizard = WizardBuilder
            .StartWith(() => new ValidatablePage(), "First")
            .Next<Unit, ValidatablePage, string>(page => page.Value).WhenValid<ValidatablePage>()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
        Assert.IsType<ValidatablePage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Next_When_builds_custom_guard_step()
    {
        var wizard = WizardBuilder
            .StartWith(() => new SimplePage(), "First")
            .Next(page => page.Value).When(page => Observable.Return(page.Value > 0))
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
    }

    [Fact]
    public void Next_with_previous_Always()
    {
        var wizard = WizardBuilder
            .StartWith(() => new SimplePage(), "First")
            .Next(page => page.Value).Always()
            .Then(prev => new SecondPage(prev), "Second")
            .Next((page, prev) => $"{prev}-{page.Suffix}").Always()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
        Assert.IsType<SimplePage>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void Next_with_previous_WhenValid()
    {
        var wizard = WizardBuilder
            .StartWith(() => new ValidatablePage(), "First")
            .Next<Unit, ValidatablePage, string>(page => page.Value).WhenValid<ValidatablePage>()
            .Then(prev => new ValidatableSecondPage(prev), "Second")
            .Next<string, ValidatableSecondPage, string>((page, prev) => $"{prev}-{page.Data}").WhenValid<ValidatableSecondPage>()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
    }

    [Fact]
    public void NextResult_Always_with_success()
    {
        var wizard = WizardBuilder
            .StartWith(() => new PageWithValidation(), "First")
            .NextResult(page => page.TryGetValue()).Always()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
        Assert.IsType<PageWithValidation>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void NextResult_WhenValid_with_failure()
    {
        var wizard = WizardBuilder
            .StartWith(() => new ValidatablePageWithResult(), "First")
            .NextResult<Unit, ValidatablePageWithResult, int>(page => page.ValidateAndProcess()).WhenValid<ValidatablePageWithResult>()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
    }

    [Fact]
    public void NextResult_with_previous_Always()
    {
        var wizard = WizardBuilder
            .StartWith(() => new SimplePage(), "First")
            .Next(page => page.Value).Always()
            .Then(prev => new PageWithCombination(prev), "Second")
            .NextResult((page, prev) => page.CombineWith(prev)).Always()
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
    }

    [Fact]
    public void NextCommand_custom_command()
    {
        var wizard = WizardBuilder
            .StartWith(() => new PageWithCustomCommand(), "First")
            .NextCommand(page => page.GetCustomCommand())
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
        Assert.IsType<PageWithCustomCommand>(wizard.CurrentPage.Content);
    }

    [Fact]
    public void NextCommand_with_previous()
    {
        var wizard = WizardBuilder
            .StartWith(() => new SimplePage(), "First")
            .Next(page => page.Value).Always()
            .Then(prev => new PageWithDependentCommand(prev), "Second")
            .NextCommand((page, prev) => page.GetCommandFor(prev))
            .WithCommitFinalStep();

        Assert.NotNull(wizard.CurrentPage);
    }
}

public class SimplePage
{
    public int Value { get; set; } = 42;
}

public class SecondPage
{
    public SecondPage(int prev)
    {
        Previous = prev;
    }

    public int Previous { get; }
    public string Suffix { get; set; } = "end";
}

public class ValidatablePage : ReactiveValidationObject, IValidatable
{
    public ValidatablePage()
    {
        this.ValidationRule(vm => vm.Value, v => !string.IsNullOrWhiteSpace(v), "Value is required");
    }

    public string Value { get; set; } = "default";
    public IObservable<bool> IsValid => ValidationContext.Valid;
}

public class ValidatableSecondPage : ReactiveValidationObject, IValidatable
{
    public ValidatableSecondPage(string prev)
    {
        Data = prev;
        this.ValidationRule(vm => vm.Data, d => !string.IsNullOrWhiteSpace(d), "Data is required");
    }

    public string Data { get; }
    public IObservable<bool> IsValid => ValidationContext.Valid;
}

public class PageWithValidation
{
    public Result<int> TryGetValue()
    {
        return Result.Success(42);
    }
}

public class ValidatablePageWithResult : ReactiveValidationObject, IValidatable
{
    public ValidatablePageWithResult()
    {
        this.ValidationRule(vm => vm.Value, v => v > 0, "Value must be positive");
    }

    public int Value { get; set; } = 10;
    public IObservable<bool> IsValid => ValidationContext.Valid;

    public Result<int> ValidateAndProcess()
    {
        return Value > 0 
            ? Result.Success(Value * 2) 
            : Result.Failure<int>("Value must be positive");
    }
}

public class PageWithCombination
{
    public PageWithCombination(int prev)
    {
        PreviousValue = prev;
    }

    public int PreviousValue { get; }

    public Result<string> CombineWith(int prev)
    {
        return Result.Success($"Combined: {prev} + {PreviousValue}");
    }
}

public class PageWithCustomCommand
{
    public IEnhancedCommand<Result<string>> GetCustomCommand()
    {
        return ReactiveCommand
            .Create(() => Result.Success("Custom result"))
            .Enhance();
    }
}

public class PageWithDependentCommand
{
    public PageWithDependentCommand(int previousValue)
    {
        PreviousValue = previousValue;
    }

    public int PreviousValue { get; }

    public IEnhancedCommand<Result<string>> GetCommandFor(int prev)
    {
        var canExecute = Observable.Return(prev > 0);
        return ReactiveCommand
            .Create(() => Result.Success($"Processed: {prev} + {PreviousValue}"), canExecute)
            .Enhance();
    }
}
