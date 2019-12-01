namespace Zafiro.PropertySystem.Tests
{
    using System.Collections.Generic;
    using Model;
    using Standard;
    using Stores;
    using Xunit;

    public class MetadataStoreTests
    {
        readonly ExtendedProperty property = new ExtendedProperty(typeof(int), "TabIndex");

        [Fact]
        public void GetSetMetadataRegisteredProperty()
        {
            var sut = CreateSut();
            var expected = new PropertyMetadata();
            sut.RegisterMetadata(typeof(Button), property, expected);
            var actual = sut.GetMetadata(typeof(Button), property);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSetMetadataUnregisteredProperty()
        {
            var sut = CreateSut();
            var expected = new PropertyMetadata();
            sut.RegisterMetadata(typeof(Button), property, expected);
            Assert.Throws<KeyNotFoundException>(() => sut.GetMetadata(typeof(Grid), property));
        }

        [Fact]
        public void GetSetMetadataDerivedClass()
        {
            var sut = CreateSut();
            var expected = new PropertyMetadata();
            sut.RegisterMetadata(typeof(Grid), property, expected);
            var actual = sut.GetMetadata(typeof(DerivedGrid), property);

            Assert.Equal(expected, actual);
        }

        private static MetadataStore<ExtendedProperty, PropertyMetadata> CreateSut()
        {
            var sut = new MetadataStore<ExtendedProperty, PropertyMetadata>();
            return sut;
        }
    }
}