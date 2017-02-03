namespace Zafiro.PropertySystem.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Model;
    using Standard;
    using Stores;
    using Xunit;

    public class ValueStoreTests
    {
        readonly ExtendedProperty property = new ExtendedProperty(typeof(int), "TabIndex");

        [Fact]
        public void GetSetMetadataRegisteredProperty()
        {
            var expected = 1234;

            var sut = CreateSut();
            var instance = new Grid();
            sut.SetValue(property, instance, expected);
            var actual = sut.GetValue(property, instance);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetOnly()
        {
            var sut = CreateSut();
            var instance = new Grid();
            Assert.Throws<KeyNotFoundException>(() => sut.GetValue(property, instance));
        }

        [Fact]
        public void Observable()
        {
            var notified = false;

            var sut = CreateSut();
            var instance = new Grid();
            var obs = sut.GetSubject(property, instance);
            obs.Subscribe(_ => notified = true);

            sut.SetValue(property, instance, 1234);
            Assert.True(notified);
        }

        private static ValueStore CreateSut()
        {
            var sut = new ValueStore();           
            return sut;
        }
    }
}