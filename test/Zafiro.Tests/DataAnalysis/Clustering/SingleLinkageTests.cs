using System.Collections.Generic;
using HeatmapPoC;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace Zafiro.Tests.DataAnalysis.Clustering;

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

    [Fact]
    public void People_cluster()
    {
        var table = Data.GetPeopleTable();
        var sut = new SingleLinkageClusteringStrategy<Data.Person>();
        Table<Cluster<Data.Person>, double> currentClusters = table.ToClusterTable();
        var cluster = sut.Clusterize(currentClusters);
        var toString = TestingHelpers.ClusterToString(cluster);

    }
}