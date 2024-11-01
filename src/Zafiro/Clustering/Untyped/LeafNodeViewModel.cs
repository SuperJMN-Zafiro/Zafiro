using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Clustering.Untyped;

public class LeafNodeViewModel : ClusterNodeViewModel
{
    public override object Content { get; }
    public override double? MergeDistance => null;
    public override IEnumerable<ClusterNodeViewModel> Children => Enumerable.Empty<ClusterNodeViewModel>();

    public LeafNodeViewModel(LeafNode<object> node, ClusterNodeViewModel parent = null)
        : base(parent)
    {
        Content = node.Item;
    }
}