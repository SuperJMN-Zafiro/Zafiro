namespace Zafiro.PropertySystem.Tests
{
    using Attached;
    using Model;
    using Xunit;

    public class AttachedEngineTests
    {
        private const int DefaultValue = 1234;
        private const int SomeValue = 10;
        private readonly AttachedPropertyEngine sut;
        private readonly AttachedProperty property;

        public AttachedEngineTests()
        {
            sut = new AttachedPropertyEngine(o => ((BaseObject)o).Parent);
            property = sut.RegisterProperty("Column", typeof(Grid), typeof(int), new AttachedPropertyMetadata { DefaultValue = DefaultValue, Inherits = true });
        }

        [Fact]
        public void AfterSet_TheSameValueIsReturned()
        {
            var instance = new Grid();
            sut.SetValue(property, instance, SomeValue);
            var actual = sut.GetValue(property, instance);

            Assert.Equal(SomeValue, actual);
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
            sut.SetValue(property, instance, SomeValue);
            var actual = sut.GetValue(property, instance);

            Assert.Equal(SomeValue, actual);
        }

        [Fact]
        public void ValueCanBeSetForAForeignClass()
        {
            var instance = new Button();

            sut.SetValue(property, instance, SomeValue);
            var val = sut.GetValue(property, instance);

            Assert.Equal(SomeValue, val);
        }

        [Fact]
        public void ValueIsInherited()
        {
            var parent = new Grid();
            var child = new Button();
            parent.AddChild(child);

            sut.SetValue(property, parent, SomeValue);
            var valueOfChild = sut.GetValue(property, child);

            Assert.Equal(SomeValue, valueOfChild);
        }

        [Fact]
        public void ChildIsNotConnectedToParent_ValueIsNotInherited()
        {
            var parent = new Grid();
            var child = new Button();

            sut.SetValue(property, parent, SomeValue);
            var valueOfChild = sut.GetValue(property, child);

            Assert.Equal(DefaultValue, valueOfChild);
        }
    }
}