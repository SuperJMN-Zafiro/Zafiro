namespace Zafiro.Clustering.Untyped;

public class LeafNodeViewModel : ClusterNodeViewModel
{
    public object Content { get; set; }

    public LeafNodeViewModel(LeafNode<object> node, ClusterNodeViewModel? parent)
        : base(parent!)
    {
        Content = node.Item;
    }
}