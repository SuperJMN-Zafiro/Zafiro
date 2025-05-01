using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Wizard;

namespace Zafiro.Tests.UI;

public class WizardTests
{
    // Helper to create a WizardStep given factories
    private WizardStep CreateStep(Func<object?, object> pageFactory, Func<object, Result<object>> onNext) => new(pageFactory, onNext);

    [Fact]
    public void Should_load_first_page_on_initialization()
    {
        // Arrange
        var firstPage = "Page1";
        var steps = new[]
        {
            CreateStep(_ => firstPage, _ => Result.Success<object>(42))
        };

        // Act
        var vm = new Wizard(steps);

        // Assert
        vm.CurrentPage.Should().Be(firstPage);
        vm.BackCommand.CanExecute.FirstAsync().Wait().Should().BeFalse();
    }

    [Fact]
    public void Next_failure_should_not_advance_or_complete()
    {
        // Arrange
        const string errorMessage = "Validation failed!";
        var steps = new[]
        {
            // Step 1 always fails
            CreateStep(_ => "Page1", _ => Result.Failure<object>(errorMessage)),
            CreateStep(_ => "Page2", _ => Result.Success<object>("OK"))
        };
        var vm = new Wizard(steps);
        bool completed = false;
        vm.WizardCompleted.Subscribe(_ => completed = true);

        // Act
        vm.NextCommand.Execute().Subscribe();

        // Assert
        vm.CurrentPage.Should().Be("Page1");
        completed.Should().BeFalse();
    }

    [Fact]
    public void Next_success_should_advance_and_emit_completion()
    {
        // Arrange
        var steps = new[]
        {
            CreateStep(_ => "Page1", _ => Result.Success<object>("A")),
            CreateStep(prev => $"Page2({prev})", _ => Result.Success<object>("Final"))
        };
        var vm = new Wizard(steps);

        object finalValue = null!;
        vm.WizardCompleted.Subscribe(val => finalValue = val);

        // Act & Assert
        vm.NextCommand.Execute().Subscribe();
        vm.CurrentPage.Should().Be("Page2(A)");

        vm.NextCommand.Execute().Subscribe();
        finalValue.Should().Be("Final");
    }

    [Fact]
    public void Back_from_second_step_should_return_to_first()
    {
        // Arrange
        var steps = new[]
        {
            CreateStep(_ => "Page1", _ => Result.Success<object>("X")),
            CreateStep(_ => "Page2", _ => Result.Success<object>("Y"))
        };
        var vm = new Wizard(steps);

        // Move forward one step
        vm.NextCommand.Execute().Subscribe();
        vm.CurrentPage.Should().Be("Page2");

        // Act
        vm.BackCommand.Execute().Subscribe();

        // Assert
        vm.CurrentPage.Should().Be("Page1");
    }

    [Fact]
    public void Should_track_page_history_correctly()
    {
        // Arrange
        var steps = new[]
        {
            CreateStep(_ => "Page1", _ => Result.Success<object>("1")),
            CreateStep(_ => "Page2", _ => Result.Success<object>("2")),
            CreateStep(_ => "Page3", _ => Result.Success<object>("3"))
        };
        var vm = new Wizard(steps);

        var visitedPages = new List<string>();
        vm.WhenAnyValue(x => x.CurrentPage)
            .Subscribe(p => visitedPages.Add((string)p!));

        // Act
        vm.NextCommand.Execute().Subscribe();
        vm.NextCommand.Execute().Subscribe();
        vm.NextCommand.Execute().Subscribe();

        // Assert
        visitedPages.Should().ContainInOrder("Page1", "Page2", "Page3");
    }
}