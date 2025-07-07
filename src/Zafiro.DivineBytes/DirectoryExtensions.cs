namespace Zafiro.DivineBytes;

public static class DirectoryExtensions
{

    public static IEnumerable<INamedByteSource> FilesRecursive(this INamedContainer container)
        => container.Resources.Concat(container.Subcontainers.SelectMany(d => d.FilesRecursive()));

    public static IEnumerable<INamedWithPath> ChildrenWithPathsRecursive(this INamedContainer container)
        => container.ChildrenRelativeTo(Path.Empty);

    public static IEnumerable<INamedByteSourceWithPath> FilesWithPathsRecursive(this INamedContainer container)
        => container.ChildrenRelativeTo(Path.Empty).OfType<INamedByteSourceWithPath>();

    public static IEnumerable<INamedWithPath> ChildrenRelativeTo(this INamedContainer container, Path path)
    {
        var myFiles = container.Resources.Select(file => new NamedByteSourceWithPath(path, file));
        var filesInSubDirs = container.Subcontainers
            .SelectMany(d => d.ChildrenRelativeTo(path.Combine(d.Name)));

        return myFiles.Concat(filesInSubDirs);
    }
    
    // public static Container Combine(string newName, IContainer a, IContainer b)
    // {
    //     var combinedChildren = new List<INamed>();
    //     var directoriesByName = new Dictionary<string, List<IContainer>>();
    //     
    //     // Primero, agrupar los subdirectorios por nombre
    //     foreach (var child in a.Children.Concat(b.Children))
    //     {
    //         if (child is IContainer dir)
    //         {
    //             if (!directoriesByName.ContainsKey(dir.Name))
    //             {
    //                 directoriesByName[dir.Name] = new List<IContainer>();
    //             }
    //             directoriesByName[dir.Name].Add(dir);
    //         }
    //         else
    //         {
    //             // Si es un archivo, agregarlo directamente
    //             combinedChildren.Add(child);
    //         }
    //     }
    //     
    //     // Ahora combinar los subdirectorios con el mismo nombre
    //     foreach (var entry in directoriesByName)
    //     {
    //         string dirName = entry.Key;
    //         List<IContainer> dirs = entry.Value;
    //         
    //         if (dirs.Count == 1)
    //         {
    //             // Si solo hay un directorio con este nombre, agregarlo directamente
    //             combinedChildren.Add(dirs[0]);
    //         }
    //         else
    //         {
    //             // If there are multiple directories with the same name, combine them recursively
    //             var combinedDir = Combine(dirName, dirs[0], dirs[1]);
    //             
    //             // Si hay más de dos, combinar los restantes
    //             for (int i = 2; i < dirs.Count; i++)
    //             {
    //                 combinedDir = Combine(dirName, combinedDir, dirs[i]);
    //             }
    //             
    //             combinedChildren.Add(combinedDir);
    //         }
    //     }
    //     
    //     return new Container(newName, combinedChildren.ToArray());
    // }
}