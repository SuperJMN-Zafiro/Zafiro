using System.Collections.Generic;
using Zafiro.Tables;

namespace Zafiro.Tests.DataAnalysis
{
    public class Data
    {
        public static Table<string, string, double> GetTable()
        {
            double[,] matrix =
            {
                {45, 30, 20, 15, 25, 35},
                {15, 25, 30, 35, 40, 20},
                {5, 10, 25, 30, 35, 15},
                {10, 15, 30, 25, 20, 10},
                {20, 35, 40, 25, 15, 10},
            };

            IList<string> columns = ["8-10", "10-12", "12-14", "14-16", "16-18", "18-20"];
            IList<string> rows = ["Café Americano", "Capuccino", "Té", "Chocolate", "Latte"];
            var sut = new Table<string, string, double>(matrix, rows, columns);
            return sut;
        }


        public static Table<Person, double> GetPeopleTable()
        {
            Person robert = new("Robert McGraw");
            Person robertSon = new("robert mcgraw");
            Person maryTeresa = new("Mary Teresa Wenck");
            Person wanda = new("Wanda Warthen");
            Person denise = new("Denise Warthen");

            List<(Person, Person, double)> edges =
            [
                (robert, maryTeresa, 1),
                (robert, robertSon, 1),
                (robert, denise, 9),
                (robert, wanda, 4),
                (maryTeresa, robertSon, 1),
                (maryTeresa, denise, 9),
                (maryTeresa, wanda, 9),
                (robertSon, denise, 9),
                (robertSon, wanda, 9),
                (denise, wanda, 1),
            ];

            return Table.FromSubsets(edges.ToArray());
        }


        public class Person(string name)
        {
            public string Name { get; } = name;
            public override string ToString()
            {
                return Name;
            }
        }
    }
}

namespace Zafiro.Tests.DataAnalysis.Tables
{

}