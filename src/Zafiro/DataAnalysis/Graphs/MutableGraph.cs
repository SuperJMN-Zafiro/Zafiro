using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.DataAnalysis.Graphs;

public class MutableGraph(IEnumerable<IMutableNode> nodes, IEnumerable<IMutableEdge> edges) : IMutableGraph, IGraph<IMutableNode, IMutableEdge>
{
    public IEnumerable<IMutableNode> Nodes { get; } = nodes;
    public IEnumerable<IWeightedEdge<IMutableNode>> Edges { get; } = edges;
    IEnumerable IGraph.Nodes => Nodes;
    IEnumerable IGraph.Edges => Edges;
    IEnumerable<IMutableEdge> IGraph<IMutableNode, IMutableEdge>.Edges => Edges.Cast<IMutableEdge>();
}