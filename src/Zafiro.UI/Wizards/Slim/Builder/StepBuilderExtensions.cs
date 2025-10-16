using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

/// <summary>
/// Convenience extensions for StepBuilder to simplify common navigation patterns.
/// </summary>
public static class StepBuilderExtensions
{
    /// <summary>
    /// Proceeds when the page is valid, executing the provided action that returns a Result.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="nextAction">Action that computes the next result from the page.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWhenValid<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, Result<TResult>> nextAction,
        string? text = "Next") where TPage : IValidatable
    {
        return builder.NextWith(page => EnhancedCommand.Create(() => nextAction(page), page.IsValid, text));
    }

    /// <summary>
    /// Proceeds when the page is valid, executing the provided action that depends on the page and previous result.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="nextAction">Action that computes the next result from the page and previous result.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWhenValid<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TPrevious, Result<TResult>> nextAction,
        string? text = "Next") where TPage : IValidatable
    {
        return builder.NextWith((page, prev) => EnhancedCommand.Create(() => nextAction(page, prev), page.IsValid, text));
    }

    /// <summary>
    /// Proceeds with a value selector and a custom can-execute gate.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page.</param>
    /// <param name="canExecute">Function that returns an observable used to gate Next.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWith<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TResult> selector,
        Func<TPage, IObservable<bool>> canExecute,
        string? text = "Next")
    {
        return builder.NextWith(page => EnhancedCommand.Create(() => Result.Success(selector(page)), canExecute(page), text));
    }

    /// <summary>
    /// Proceeds with a value selector, automatically gating Next with IValidatable.IsValid.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWith<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TResult> selector,
        string? text = "Next")
    {
        return builder.NextWith(page =>
        {
            var canExecute = page is IValidatable validatable
                ? validatable.IsValid
                : Observable.Return(true);

            return EnhancedCommand.Create(() => Result.Success(selector(page)), canExecute, text);
        });
    }

    /// <summary>
    /// Proceeds when the page is valid (IValidatable.IsValid), using a selector that returns a value directly.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWhenValid<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TResult> selector,
        IObservable<bool>? isValid = null,
        string? text = "Next") where TPage : IValidatable
    {
        return builder.NextWith(page => EnhancedCommand.Create(() => Result.Success(selector(page)), isValid ?? page.IsValid, text));
    }


    /// <summary>
    /// Proceeds with a value selector and no guard (always enabled).
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextAlways<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TResult> selector,
        string? text = "Next")
    {
        return builder.NextWith(page => EnhancedCommand.Create(() => Result.Success(selector(page)), Observable.Return(true), text));
    }

    /// <summary>
    /// Proceeds unconditionally yielding Unit, useful for final steps like "Close".
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<Unit> NextAlways<TPrevious, TPage>(
        this StepBuilder<TPrevious, TPage> builder,
        string? text = "Next")
    {
        return builder.NextAlways<TPrevious, TPage, Unit>(_ => Unit.Default, text);
    }

    /// <summary>
    /// Proceeds when the page is valid (IValidatable.IsValid) using a selector that depends on both the page and the previous result.
    /// You can optionally supply a custom validity observable to override the default.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page and previous result.</param>
    /// <param name="isValid">Optional validity observable. Defaults to page.IsValid when null.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWhenValid<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TPrevious, TResult> selector,
        IObservable<bool>? isValid = null,
        string? text = "Next") where TPage : IValidatable
    {
        return builder.NextWith((page, prev) => EnhancedCommand.Create(() => Result.Success(selector(page, prev)), isValid ?? page.IsValid, text));
    }

    /// <summary>
    /// Proceeds with a value selector that depends on both page and previous result, using a custom can-execute gate.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page and previous result.</param>
    /// <param name="canExecute">Function that builds the can-execute observable from the page and previous result.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWith<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TPrevious, TResult> selector,
        Func<TPage, TPrevious, IObservable<bool>> canExecute,
        string? text = "Next")
    {
        return builder.NextWith((page, prev) => EnhancedCommand.Create(() => Result.Success(selector(page, prev)), canExecute(page, prev), text));
    }

    /// <summary>
    /// Proceeds with a value selector that depends on both page and previous result, automatically gating with IValidatable.IsValid.
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type, which must be validatable.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page and previous result.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextWith<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TPrevious, TResult> selector,
        string? text = "Next") where TPage : IValidatable
    {
        return builder.NextWith((page, prev) => EnhancedCommand.Create(() => Result.Success(selector(page, prev)), page.IsValid, text));
    }

    /// <summary>
    /// Proceeds unconditionally with a selector that depends on page and previous result (always enabled).
    /// </summary>
    /// <typeparam name="TPrevious">The type of the previous step result.</typeparam>
    /// <typeparam name="TPage">The current page type.</typeparam>
    /// <typeparam name="TResult">The type produced by the next step.</typeparam>
    /// <param name="builder">The step builder.</param>
    /// <param name="selector">Selector that produces the next result from the page and previous result.</param>
    /// <param name="text">Optional button text. Defaults to "Next".</param>
    public static WizardBuilder<TResult> NextAlways<TPrevious, TPage, TResult>(
        this StepBuilder<TPrevious, TPage> builder,
        Func<TPage, TPrevious, TResult> selector,
        string? text = "Next")
    {
        return builder.NextWith((page, prev) => EnhancedCommand.Create(() => Result.Success(selector(page, prev)), Observable.Return(true), text));
    }
}