using System.Collections.Generic;

namespace Zafiro.Clustering.Untyped;
public class InternalNodeViewModel : ClusterNodeViewModel
{
    public override object Content => "Internal Node";
    public override double? MergeDistance { get; }
    public ClusterNodeViewModel Left { get; }
    public ClusterNodeViewModel Right { get; }
    public override IEnumerable<ClusterNodeViewModel> Children => new[] { Left, Right };

    public InternalNodeViewModel(InternalNode<object> node, ClusterNodeViewModel parent = null)
        : base(parent)
    {
        MergeDistance = node.MergeDistance;
        Left = Factory.CreateViewModel(node.Left, this);
        Right = Factory.CreateViewModel(node.Right, this);
    }
}
