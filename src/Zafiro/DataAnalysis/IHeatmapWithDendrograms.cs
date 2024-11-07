using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis;

public interface IHeatmapWithDendrograms
{
    public ITable Table { get; }
    public ICluster RowsCluster { get; }
    public ICluster ColumnsCluster { get; }
}