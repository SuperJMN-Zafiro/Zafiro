using System.Collections;
using System.Collections.Generic;

namespace Zafiro.DataAnalysis.Graphs;

public interface IGraph<out TNode, out TEdge> : IGraph where TEdge : IEdge<TNode>
{
    new IEnumerable<TNode> Nodes { get; }
    new IEnumerable<TEdge> Edges { get; }
}

public interface IGraph
{
    IEnumerable Nodes { get; }
    IEnumerable Edges { get; }
}