using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Zafiro.Core.UI
{
    public static class DialogMixin
    {
        public static IDisposable HandleExceptionsFromCommand<TInput, TOutput>(this IDialogService dialogService,
            ReactiveCommand<TInput, TOutput> command, string title = null, string message = null)
        {
            return command.ThrownExceptions
                .SelectMany(exception =>
                {
                    return Observable.FromAsync(
                        () => dialogService.Show(title ?? "An error has occurred", message ?? exception.Message));
                }).Subscribe();
        }
    }
}