using Zafiro.DataAnalysis;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tests.DataAnalysis;

namespace Zafiro.Tests.Tables;

public class IntegrationTests
{
    [Fact]
    public void Doit()
    {
        var table = Data.GetTable();
        var clustering = new SingleLinkageClusteringStrategy<string>();
        var heatmap = HeatmapWithDendrograms.Create(table, clustering, clustering);
    }
}