using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using ReactiveUI;
using Xunit;
using Zafiro.Core.UI.Interaction;

namespace Zafiro.Wpf.Tests
{
    public class ShellTests
    {
        [Fact]
        public async Task PopupClose()
        {
            var popupMock = new PopupMock();
            IDisposable subscription = null;
            
            var sut = new Shell(() => popupMock);
            await sut.Popup(new Contextualized(new object()), new SampleViewModel(), config =>
            {
                var close = ReactiveCommand.Create(() => config.Popup.Close());
                config.AddOption(new Option("Salute", close));
                subscription = config.Popup.Shown.InvokeCommand(close);
            });

            subscription.Dispose();
        }

        [Fact]
        public async Task Validation()
        {
            var popupMock = new PopupMock();
            CompositeDisposable disposables = new CompositeDisposable();
            
            var sut = new Shell(() => popupMock);
            var sampleViewModel = new SampleViewModel();
            await sut.Popup(new Contextualized(new object()), sampleViewModel, config =>
            {
                var execute = ReactiveCommand.Create(() => config.Model.Executed(), config.Model.IsEven);
                config.AddOption(new Option("Execute", execute));
                config.Popup.Shown.Subscribe(_ => config.Popup.Close())
                    .DisposeWith(disposables);
                execute.Execute(Unit.Default).Subscribe().DisposeWith(disposables);
            });

            disposables.Dispose();

            sampleViewModel.IsExecuted.Should().BeTrue();
        }
    }
}