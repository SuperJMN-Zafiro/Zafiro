namespace Zafiro.PropertySystem.Tests
{
    using System;
    using Model;
    using Standard;
    using Xunit;

    public class StandardEngineTests
    {
        private const int DefaultValue = 1234;
        private readonly ExtendedPropertyEngine sut;
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
        public void CannotSetValueOfPropertyOfNotRegisteredClass()
        {
            var instance = new Button();

            Assert.Throws<InvalidOperationException>(() => sut.SetValue(property, instance, 10));
        }      
    }
}
