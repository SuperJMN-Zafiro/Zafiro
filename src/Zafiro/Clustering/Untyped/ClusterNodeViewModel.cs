using System.Collections.Generic;

namespace Zafiro.Clustering.Untyped;

public abstract class ClusterNodeViewModel
{
    public ClusterNodeViewModel Parent { get; }
    public abstract object Content { get; }
    public abstract double? MergeDistance { get; }
    public abstract IEnumerable<ClusterNodeViewModel> Children { get; }

    protected ClusterNodeViewModel(ClusterNodeViewModel parent)
    {
        Parent = parent;
    }
}