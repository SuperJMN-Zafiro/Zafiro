using System;
using Zafiro.Tables;

namespace Zafiro.Tests.DataAnalysis.Tables;

public class TableTests
{
    [Fact]
    public void Test1()
    {
        var bidim = Data.GetTable();
        var val = bidim.Get("Café Americano", "12-14");
        val.Should().Be(20);
    }

    [Fact]
    public void Get_row_distances()
    {
        var m = Data.GetTable();
        var distances = m.ToRowDistances();
        string toTable = distances.ToString();
    }

    private double Distance(int a, int b)
    {
        var sumOfSquares = Math.Pow(a - b, 2);
        return Math.Sqrt(sumOfSquares);
    }
}