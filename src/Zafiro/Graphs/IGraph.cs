using System.Collections.Generic;

namespace Zafiro.Graphs;

public interface IGraph<out TNode, out TEdge> where TEdge : IEdge<TNode>
{
    IEnumerable<TNode> Nodes { get; }
    IEnumerable<TEdge> Edges { get; }
}