using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public class SingleLinkageClusteringStrategy<T> : IClusteringStrategy<T>
{
    public Cluster<T> Clusterize(Table<Cluster<T>, double> table)
    {
        return Clusterize(table.ColumnLabels, table);
    }

    public Cluster<T> Clusterize(IList<Cluster<T>> currentClusters, Table<Cluster<T>, double> currentClustersDistances)
    {
        if (currentClusters.Count == 1)
        {
            return currentClusters.First();
        }

        var closestPair = ClosestPair(currentClusters, currentClustersDistances);
        var newCluster = new Internal<T>(closestPair.A, closestPair.B, closestPair.Distance);
        var withPairExcluded = currentClusters.Except([closestPair.A, closestPair.B]).ToList();

        var clusters = new List<Cluster<T>> { newCluster }.Concat(withPairExcluded).ToList();
        var distances = DistancesFor(currentClustersDistances, withPairExcluded, newCluster);

        return Clusterize(clusters, distances);
    }

    private static Table<Cluster<T>, double> DistancesFor(Table<Cluster<T>, double> previousTable,
        IList<Cluster<T>> actual, Cluster<T> added)
    {
        if (!actual.Any())
        {
            return previousTable;
        }

        var all = new List<Cluster<T>> { added }.Concat(actual);

        var subsets = all.Subsets(2).Select(x =>
        {
            var a = x[0];
            var b = x[1];

            double distance;

            if (Equals(a, added) && a is Internal<T> addedInternalA)
            {
                // Si `a` es el nodo añadido y es un InternalNode
                distance = Math.Min(previousTable.Get(b, addedInternalA.Left), previousTable.Get(b, addedInternalA.Right));
            }
            else if (Equals(b, added) && b is Internal<T> addedInternalB)
            {
                // Si `b` es el nodo añadido y es un InternalNode
                distance = Math.Min(previousTable.Get(a, addedInternalB.Left), previousTable.Get(a, addedInternalB.Right));
            }
            else
            {
                // En caso de que ninguno sea el nodo añadido o que no sea un InternalNode
                distance = previousTable.Get(a, b);
            }

            return (a, b, distance);
        }).ToList();

        var table = subsets.ToTable();

        return table;
    }


    private static (Cluster<T> A, Cluster<T> B, double Distance) ClosestPair(IEnumerable<Cluster<T>> clusters, Table<Cluster<T>, Cluster<T>, double> distances)
    {
        var combinations = clusters.Subsets(2);
        var tuples = from n in combinations
                     let left = n[0]
                     let right = n[1]
                     let distance = distances.Get(left, right)
                     orderby distance
                     select (left, right, distance);

        var closest = tuples.First();

        return closest;
    }
}