using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Core.Mixins;
using FluentAssertions;
using Mapping;
using Xunit;

namespace XPath.Tests
{
    public class XpathMapperTests
    {
        [Fact]
        public void Map_sample_model_should_be_expected()
        {
            using var stream = File.OpenRead("SampleFiles\\Simple.xml");
            var mapper = new Configuration(c =>
            {
                c.CreateMap<SomeModel>()
                    .ForMember(x => x.Text, e => e.MapFrom("/Root/Text"))
                    .ForMember(x => x.Int, e => e.MapFrom("/Root/Int", int.Parse))
                    .ForMember(x => x.Combined, e => e.MapFrom("/Root/Int", "/Root/Text", (a, b) => a + b))
                    .ForMember(x => x.Children, e => e.MapFrom("//Child"))
                    .ForMember(x => x.Strings, e => e.MapFrom("//Strings/String"))
                    .ForMember(x => x.Ints, e => e.MapFrom("//Numbers/Number"));
                c.CreateMap<Child>()
                    .ForMember(x => x.Name, x => x.MapFrom("Name"));

            }).CreateMapper();

            var mapped = mapper.Map<SomeModel>(new XPathDocument(stream));

            var someModel = new SomeModel
            {
                Int = 123,
                Text = "Hola",
                Combined = "123Hola",
                Children = new List<Child>
                {
                    new() {Name = "Hi1"},
                    new() {Name = "Hi2"},
                    new() {Name = "Hi3"}
                },
                Strings = new[] {"Hola", "Cómo", "Estás"}.ToList(),
                Ints = new[] {1, 2, 3}.ToList()
            };

            mapped.Should().BeEquivalentTo(someModel);
        }

        [Fact]
        public void Fallback_value_with_one_source()
        {
            var doc = new XPathDocument("<Root></Root>".ToStream());

            var mapper = new Configuration(c =>
            {
                c.CreateMap<SomeModel>()
                    .ForMember(x => x.Int, e => e.MapFrom("/Root/Missing", _ => default).UseFallbackValue(123));
            }).CreateMapper();

            var model = mapper.Map<SomeModel>(doc);
            model.Int.Should().Be(123);
        }

        [Fact]
        public void Fallback_value_with_several_sources()
        {
            var doc = new XPathDocument("<Root><Int>1</Int></Root>".ToStream());

            var mapper = new Configuration(c =>
            {
                c.CreateMap<SomeModel>()
                    .ForMember(x => x.Int, e => e.MapFrom("Int", "/Root/Missing", (_, _) => default).UseFallbackValue(123));
            }).CreateMapper();

            var model = mapper.Map<SomeModel>(doc);
            model.Int.Should().Be(123);
        }

        [Fact]
        public void Fallback_value_with_several_sources_should_not_be_applied_when_all_are_found()
        {
            var doc = new XPathDocument("<Root><Int>1</Int><AnotherInt>2</AnotherInt></Root>".ToStream());

            var mapper = new Configuration(c =>
            {
                c.CreateMap<SomeModel>()
                    .ForMember(x => x.Int, e => e.MapFrom("/Root/Int", "/Root/AnotherInt", (_, _) => 123).UseFallbackValue(404));
            }).CreateMapper();

            var model = mapper.Map<SomeModel>(doc);
            model.Int.Should().Be(123);
        }
    }
}
