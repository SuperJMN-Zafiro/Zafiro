using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zafiro;

namespace Zafiro.DataAnalysis.Graphs;

public static class GraphExtensions
{
    private static class ObjectCache
    {
        private static readonly ConditionalWeakTable<object, Dictionary<object, object>> Calculations = new();

        public static object GetOrCalculate(object owner, object key, Func<object> calculation)
        {
            var cache = Calculations.GetOrCreateValue(owner);
            if (cache.TryGetValue(key, out var cachedValue))
            {
                return cachedValue;
            }

            var result = calculation();
            cache[key] = result;

            return result;
        }
    }

    public static double RelativeDegreeCentrality<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node) where TEdge : IWeightedEdge<TNode>
    {
        return (double)ObjectCache.GetOrCalculate(graph, (nameof(RelativeDegreeCentrality), node), () => graph.RelativeDegreeCentralityCore(node));
    }

    private static double RelativeDegreeCentralityCore<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IWeightedEdge<TNode>
    {
        var nodeImportance = graph.DegreeCentrality(node);
        var maxImportance = graph.MaxDegreeCentrality();

        return maxImportance > 0 ? nodeImportance / maxImportance : 0;
    }


    public static double MaxDegreeCentrality<TNode, TEdge>(this IGraph<TNode, TEdge> graph)
        where TEdge : IWeightedEdge<TNode>
    {
        return (double)ObjectCache.GetOrCalculate(graph, nameof(MaxDegreeCentrality), () => graph.MaxDegreeCentralityCore());
    }

    private static double MaxDegreeCentralityCore<TNode, TEdge>(this IGraph<TNode, TEdge> graph)
        where TEdge : IWeightedEdge<TNode>
    {
        if (!graph.Nodes.Any()) return 0;

        return graph.Nodes.Max(node => graph.DegreeCentrality(node));
    }

    public static double DegreeCentrality<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IWeightedEdge<TNode>
    {
        return (double)ObjectCache.GetOrCalculate(graph, (nameof(DegreeCentrality), node), () => graph.DegreeCentralityCore(node));
    }

    private static double DegreeCentralityCore<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IWeightedEdge<TNode>
    {
        var adjacent = graph.AdjacentEdges(node);
        var degreeCentrality = adjacent.Sum(edge => edge.Weight);
        return degreeCentrality;
    }

    public static IEnumerable<TEdge> AdjacentEdges<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IWeightedEdge<TNode>
    {
        return (IEnumerable<TEdge>)ObjectCache.GetOrCalculate(graph, (nameof(AdjacentEdges), node), () => graph.AdjacentEdgesCore(node));
    }

    private static List<TEdge> AdjacentEdgesCore<TNode, TEdge>(this IGraph<TNode, TEdge> graph, TNode node)
        where TEdge : IWeightedEdge<TNode>
    {
        return graph.Edges.Where(edge => Equals(edge.From, node)).ToList();
    }
}