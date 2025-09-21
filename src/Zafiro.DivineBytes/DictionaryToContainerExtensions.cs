using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

public static class DictionaryToContainerExtensions
{
    /// <summary>
    /// Converts a dictionary of file paths and their byte sources into a directory tree structure.
    /// </summary>
    /// <param name="pathContentDictionary">Dictionary where keys are file paths and values are IByteSource content</param>
    /// <param name="rootName">Name for the root directory (optional, defaults to "root")</param>
    /// <returns>A Result containing the root Directory or an error message</returns>
    public static Result<Container> ToDirectoryTree(this Dictionary<string, IByteSource> pathContentDictionary, string rootName = "")
    {
        try
        {
            if (pathContentDictionary.Count == 0)
            {
                return Result.Success(new Container(rootName));
            }

            var rootChildren = new List<INamed>();
            var directoryGroups = new Dictionary<string, List<(string remainingPath, IByteSource content)>>();

            foreach (var kvp in pathContentDictionary)
            {
                var path = kvp.Key.Trim('/');
                var content = kvp.Value;

                if (string.IsNullOrEmpty(path))
                {
                    continue; // Skip empty paths
                }

                var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (pathParts.Length == 1)
                {
                    // This is a file directly in the root
                    var file = new Resource(pathParts[0], content);
                    rootChildren.Add(file);
                }
                else
                {
                    // This is a nested path, group by first directory
                    var firstDir = pathParts[0];
                    var remainingPath = string.Join("/", pathParts.Skip(1));

                    if (!directoryGroups.ContainsKey(firstDir))
                    {
                        directoryGroups[firstDir] = new List<(string, IByteSource)>();
                    }

                    directoryGroups[firstDir].Add((remainingPath, content));
                }
            }

            // Process directory groups recursively
            foreach (var group in directoryGroups)
            {
                var subDictionary = group.Value.ToDictionary(
                    item => item.remainingPath,
                    item => item.content
                );

                var subDirectoryResult = subDictionary.ToDirectoryTree(group.Key);
                if (subDirectoryResult.IsFailure)
                {
                    return Result.Failure<Container>($"Failed to create subdirectory '{group.Key}': {subDirectoryResult.Error}");
                }

                rootChildren.Add(subDirectoryResult.Value);
            }

            return Result.Success(new Container(rootName, rootChildren.ToArray()));
        }
        catch (Exception ex)
        {
            return Result.Failure<Container>($"Error creating directory tree: {ex.Message}");
        }
    }
}