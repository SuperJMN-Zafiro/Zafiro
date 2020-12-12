using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using ReactiveUI;
using Xunit;
using Zafiro.UI;

namespace Zafiro.Wpf.Tests
{
    public class ShellTests
    {
        [Fact]
        public async Task PopupClose()
        {
            var popupMock = new ViewMock();
            IDisposable subscription = null;
            
            var sut = new Popup(() => popupMock);
            await sut.ShowAsModal(new TestHaveDataContext(), new SampleViewModel(), config =>
            {
                var close = ReactiveCommand.Create(() => config.View.Close());
                config.AddOption(new Option("Salute", close));
                subscription = config.View.Shown.InvokeCommand(close);
            });

            subscription.Dispose();
        }

        [Fact]
        public async Task Validation()
        {
            var popupMock = new ViewMock();
            CompositeDisposable disposables = new CompositeDisposable();
            
            var sut = new Popup(() => popupMock);
            var sampleViewModel = new SampleViewModel();
            await sut.ShowAsModal(new TestHaveDataContext(), sampleViewModel, config =>
            {
                var execute = ReactiveCommand.Create(() => config.Model.Executed(), config.Model.IsEven);
                config.AddOption(new Option("Execute", execute));
                config.View.Shown.Subscribe(_ => config.View.Close())
                    .DisposeWith(disposables);
                execute.Execute(Unit.Default).Subscribe().DisposeWith(disposables);
            });

            disposables.Dispose();

            sampleViewModel.IsExecuted.Should().BeTrue();
        }
    }
}