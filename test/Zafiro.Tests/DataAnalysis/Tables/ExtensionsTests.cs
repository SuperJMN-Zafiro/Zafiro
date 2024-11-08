namespace Zafiro.Tests.DataAnalysis.Tables;

public class ExtensionsTests
{
    [Fact]
    public void CreateFromDistances()
    {
        var matrix = Data.GetPeopleTable();

        var expected = """
                                          Robert McGraw  Mary Teresa Wenck  robert mcgraw  Denise Warthen  Wanda Warthen  
                       Robert McGraw      0              1                  1              9               4              
                       Mary Teresa Wenck  1              0                  1              9               9              
                       robert mcgraw      1              1                  0              9               9              
                       Denise Warthen     9              9                  9              0               1              
                       Wanda Warthen      4              9                  9              1               0              

                       """;

        matrix.ToString().Should().Be(expected);
    }
}