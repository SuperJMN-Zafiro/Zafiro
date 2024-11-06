using System.Collections.Generic;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace HeatmapPoC;

public class SingleLinkageTests
{
    [Fact]
    public void Test()
    {
        var sut = new SingleLinkageClusteringStrategy<string>();
        IList<Cluster<string>> myClusters = [
            new Leaf<string>("A"), 
            new Leaf<string>("B"),
            new Leaf<string>("C")
        ];

        var doubles = new double[,]
        {
            {0, 3, 5},
            {3, 0, 2},
            {5, 2, 0},
        };

        var actual = sut.Clusterize(myClusters, new Table<Cluster<string>, double>(doubles, myClusters));

        var expected = """
                       Cluster:
                         FusionDistance: 3
                         Left:
                           Cluster:
                             FusionDistance: 2
                             Left:
                               Cluster:
                                 Content: B
                             Right:
                               Cluster:
                                 Content: C
                         Right:
                           Cluster:
                             Content: A

                       """;

        var actualStr = TestingHelpers.ClusterToString(actual);
        actualStr.Should().Be(expected);
    }
}