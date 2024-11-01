using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Zafiro.Clustering;

public class SingleLinkageClustering<T, TValue> where TValue : IComparable<TValue>
{
    public ClusterNode<T> Clusterize(Table<ClusterNode<T>, double> table)
    {
        return Clusterize(table.ColumnLabels, table);
    }

    public ClusterNode<T> Clusterize(IList<ClusterNode<T>> currentClusters, Table<ClusterNode<T>, double> currentClustersDistances)
    {
        if (currentClusters.Count == 1)
        {
            return currentClusters.First();
        }

        var closestPair = ClosestPair(currentClusters, currentClustersDistances);
        var newCluster = new InternalNode<T>(closestPair.A, closestPair.B, closestPair.Distance);
        var withPairExcluded = currentClusters.Except([closestPair.A, closestPair.B]).ToList();

        var clusters = new List<ClusterNode<T>> { newCluster }.Concat(withPairExcluded).ToList();
        var distances = DistancesFor(currentClustersDistances, withPairExcluded, newCluster);

        return Clusterize(clusters, distances);
    }

    private Table<ClusterNode<T>, double> DistancesFor(Table<ClusterNode<T>, double> previousTable,
        IList<ClusterNode<T>> actual, ClusterNode<T> added)
    {
        if (!actual.Any())
        {
            return previousTable;
        }

        var all = new List<ClusterNode<T>> { added }.Concat(actual);

        var subsets = all.Subsets(2).Select(x =>
        {
            var a = x[0];
            var b = x[1];

            double distance;

            if (Equals(a, added) && a is InternalNode<T> addedInternalA)
            {
                // Si `a` es el nodo añadido y es un InternalNode
                distance = Math.Min(previousTable.Get(b, addedInternalA.Left), previousTable.Get(b, addedInternalA.Right));
            }
            else if (Equals(b, added) && b is InternalNode<T> addedInternalB)
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

        var table = Table.FromSubsets(subsets.ToArray());

        return table;
    }


    private static (ClusterNode<T> A, ClusterNode<T> B, double Distance) ClosestPair(IEnumerable<ClusterNode<T>> clusters, LabeledTable<ClusterNode<T>, ClusterNode<T>, double> distances)
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