namespace Zafiro.DivineBytes;

public class Container : IContainer
{
    public string Name { get; }
    public IEnumerable<INamed> Children { get; }
    
    // Constructor privado para la implementación interna
    private Container(string name, IEnumerable<INamed> children)
    {
        Name = name;
        Children = children.ToList();
    }

    public Container(string name, IEnumerable<INamedByteSource> files, IEnumerable<IContainer> directories)
    {
        Name = name;
        Children = files.Cast<INamed>().Concat(directories).ToList();
    }
    
    // Método de fábrica estático que permite sintaxis fluida
    public static Container Create(string name, params INamed[] contents)
    {
        return new Container(name, contents);
    }
    
    // Constructor público que permite la sintaxis sugerida
    public Container(string name, params INamed[] contents)
    {
        Name = name;
        Children = contents.ToList();
    }
    
    // Métodos para mostrar la estructura
    public override string ToString()
    {
        return $"Directory: {Name} ({Children.Count()} items)";
    }
}