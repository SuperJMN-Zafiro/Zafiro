using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using FluentAssertions;
using Xunit;
using Zafiro.Mapping;

namespace Zafiro.XPath.Tests
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
    }
}
