namespace Zafiro.Clustering.Untyped;
public class InternalNodeViewModel : ClusterNodeViewModel
{
    public ClusterNodeViewModel Left { get; }
    public ClusterNodeViewModel Right { get; }

    public InternalNodeViewModel(InternalNode<object> node, ClusterNodeViewModel parent = null)
        : base(parent)
    {
        Left = CreateViewModel(node.Left, this);
        Right = CreateViewModel(node.Right, this);
    }
}
