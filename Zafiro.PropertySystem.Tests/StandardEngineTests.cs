namespace Zafiro.PropertySystem.Tests
{
    using System;
    using Model;
    using Standard;
    using Xunit;

    public class StandardEngineTests
    {
        private const int DefaultValue = 1234;
        private readonly IExtendedPropertyEngine sut;
        private readonly ExtendedProperty property;

        public StandardEngineTests()
        {
            sut = new ExtendedPropertyEngine();
            property = sut.RegisterProperty("IntProperty", typeof(Grid), typeof(int), new PropertyMetadata() { DefaultValue = DefaultValue });
        }

        [Fact]
        public void AfterSet_TheSameValueIsReturned()
        {
            var instance = new Grid();
            sut.SetValue(property, instance, 10);
            var actual = sut.GetValue(property, instance);

            Assert.Equal(10, actual);
        }

        [Fact]
        public void IfNoValueIsSet_DefaultValueIsReturned()
        {
            var instance = new Grid();
            var actual = sut.GetValue(property, instance);

            Assert.Equal(DefaultValue, actual);
        }

        [Fact]
        public void AfterSetInDerivedClass_TheSameValueIsReturned()
        {
            var instance = new DerivedGrid();
            sut.SetValue(property, instance, 10);
            var actual = sut.GetValue(property, instance);

            Assert.Equal(10, actual);
        }

        [Fact]
        public void PushingTheSameValue_NotificationIsOnlyReceivedOnce()
        {
            var count = 0;
            var instance = new Grid();
            sut.GetChangedObservable(property, instance).Subscribe(o => count++);
            sut.SetValue(property, instance, 10);
            sut.SetValue(property, instance, 10);

            Assert.Equal(2, count);
        }

        [Fact]
        public void GettingDefaultValue_PushesDefaultValue()
        {
            var count = 0;
            var instance = new Grid();
            sut.GetChangedObservable(property, instance).Subscribe(o => count++);
            sut.GetValue(property, instance);

            Assert.Equal(1, count);
        }

        [Fact]
        public void CannotSetValueOfPropertyOfNotRegisteredClass()
        {
            var instance = new Button();

            Assert.Throws<InvalidOperationException>(() => sut.SetValue(property, instance, 10));
        }      
    }
}
