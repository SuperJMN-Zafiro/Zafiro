using System.Text;
using Zafiro.DataAnalysis.Clustering;

namespace HeatmapPoC;

public static class TestingHelpers
{
    public static string ClusterToString<T>(Cluster<T> cluster) where T : class
    {
        return ClusterToString(cluster, 0);
    }

    private static string ClusterToString<T>(Cluster<T> cluster, int indentLevel) where T : class
    {
        var indent = new string(' ', indentLevel * 2);
        var sb = new StringBuilder();

        sb.AppendLine($"{indent}Cluster:");

        if (cluster is Leaf<T> leaf)
        {
            sb.AppendLine($"{indent}  Content: {leaf.Item}");
        }
        else if (cluster is Internal<T> intermediate)
        {
            sb.AppendLine($"{indent}  FusionDistance: {intermediate.MergeDistance}");

            sb.AppendLine($"{indent}  Left:");
            sb.Append(ClusterToString(intermediate.Left, indentLevel + 2));

            sb.AppendLine($"{indent}  Right:");
            sb.Append(ClusterToString(intermediate.Right, indentLevel + 2));
        }

        return sb.ToString();
    }
}