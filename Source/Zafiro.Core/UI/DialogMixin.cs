using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace Zafiro.Core.UI
{
    public static class DialogMixin
    {
        public static IDisposable HandleExceptionsFromCommand(this IDialogService dialogService,
            IReactiveCommand command, string title = null, string message = null)
        {
            return command.ThrownExceptions
                .SelectMany(exception =>
                {
                    return Observable.FromAsync(
                        () => dialogService.Show(title ?? "An error has occurred", message ?? exception.Message));
                }).Subscribe();
        }

        public static IDisposable HandleExceptionsFromCommand(this IDialogService dialogService,
            IReactiveCommand command, Func<Exception, (string Title, string Message)> handler)
        {
            return command.ThrownExceptions
                .SelectMany(exception =>
                {
                    var props = handler(exception);
                    return Observable.FromAsync(
                        () => dialogService.Show(props.Title, props.Message));
                }).Subscribe();
        }
    }
}