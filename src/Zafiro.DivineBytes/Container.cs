namespace Zafiro.DivineBytes;

public class Container : INamedContainer
{
    public string Name { get; }
    public IEnumerable<INamedContainer> Subcontainers { get; }
    public IEnumerable<INamedByteSource> Resources { get; }

    public Container(string name, IEnumerable<INamedByteSource> files, IEnumerable<INamedContainer> directories)
    {
        Name = name;
        Resources = files.ToList();
        Subcontainers = directories.ToList();
    }

    // Static factory method for fluent syntax
    public static Container Create(string name, IEnumerable<INamedByteSource> resources, IEnumerable<INamedContainer> subcontainers)
    {
        return new Container(name, resources, subcontainers);
    }

    // Constructor that accepts mixed content and separates by type
    public Container(string name, params INamed[] contents)
    {
        Name = name;
        Resources = contents.OfType<INamedByteSource>().ToList();
        Subcontainers = contents.OfType<INamedContainer>().ToList();
    }

    // Method to show structure
    public override string ToString()
    {
        return $"Directory: {Name} ({Resources.Count()} files, {Subcontainers.Count()} subdirs)";
    }
}