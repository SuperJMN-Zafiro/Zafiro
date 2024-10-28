using System.Collections.Generic;

namespace Zafiro.Graphs;

public class Graph<TNode, TEdge>(IEnumerable<TNode> nodes, IEnumerable<TEdge> edges) where TEdge : IEdge<TNode>
{
    public IEnumerable<TNode> Nodes { get; } = nodes;
    public IEnumerable<TEdge> Edges { get; } = edges;
}