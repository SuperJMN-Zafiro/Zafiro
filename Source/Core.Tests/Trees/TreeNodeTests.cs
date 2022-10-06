using System.Collections.Generic;
using System.Linq;
using Core.Trees;
using FluentAssertions;
using Xunit;

namespace Core.Tests.Trees;

public class TreeNodeTests
{
    [Fact]
    public void Path_should_be_retrieved()
    {
        // Arrange
        var toLocate = new Model("ChildX", Enumerable.Empty<Model>());
        var tree = new Model("Root",
            new List<Model>
            {
                new("Child1", Enumerable.Empty<Model>()),
                new("Child2", new List<Model>
                {
                    new("Child3", Enumerable.Empty<Model>()),
                    new("Child4", Enumerable.Empty<Model>()),
                    toLocate
                })
            });

        // Act
        var path = tree.GetPath(toLocate, x => x.Children);

        // Assert
        path.Should().BeEquivalentTo(new[] {0, 1, 2});
    }
}