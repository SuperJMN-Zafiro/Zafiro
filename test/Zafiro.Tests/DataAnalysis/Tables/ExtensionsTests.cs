using Zafiro.Tables;

namespace HeatmapPoC;

public class ExtensionsTests
{
    [Fact]
    public void CreateFromDistances()
    {
        var matrix = Table.FromSubsets(("A", "B", 12));
    }
}