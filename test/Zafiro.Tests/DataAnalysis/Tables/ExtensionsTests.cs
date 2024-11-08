namespace Zafiro.Tests.DataAnalysis.Tables;

public class ExtensionsTests
{
    [Fact]
    public void CreateFromDistances()
    {
        var matrix = Data.GetPeopleTable();

        var expected = """
                               Robert  Mary  John  Denise  Wanda  
                       Robert  0       1     1     9       4      
                       Mary    1       0     1     9       9      
                       John    1       1     0     9       9      
                       Denise  9       9     9     0       1      
                       Wanda   4       9     9     1       0      
                       
                       """;

        matrix.ToString().Should().Be(expected);
    }
}