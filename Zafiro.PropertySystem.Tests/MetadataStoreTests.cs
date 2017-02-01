namespace Zafiro.PropertySystem.Tests
{
    using System.Collections.Generic;
    using Model;
    using PropertySystem;
    using Standard;
    using UnitTestProject2;
    using Xunit;

    public class MetadataStoreTests
    {
        readonly ExtendedProperty property = new ExtendedProperty(typeof(int), "TabIndex");

        [Fact]
        public void GetSetMetadataRegisteredProperty()
        {
            var sut = CreateSut();
            var expected = new Metadata();
            sut.RegisterMetadata(typeof(Button), property, expected);
            var actual = sut.GetMetadata(typeof(Button), property);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetSetMetadataUnregisteredProperty()
        {
            var sut = CreateSut();
            var expected = new Metadata();
            sut.RegisterMetadata(typeof(Button), property, expected);
            Assert.Throws<KeyNotFoundException>(() => sut.GetMetadata(typeof(Grid), property));
        }

        [Fact]
        public void GetSetMetadataDerivedClass()
        {
            var sut = CreateSut();
            var expected = new Metadata();
            sut.RegisterMetadata(typeof(Grid), property, expected);
            var actual = sut.GetMetadata(typeof(DerivedGrid), property);

            Assert.Equal(expected, actual);
        }

        private static MetadataStore<ExtendedProperty, Metadata> CreateSut()
        {
            var sut = new MetadataStore<ExtendedProperty, Metadata>();
            return sut;
        }
    }
}