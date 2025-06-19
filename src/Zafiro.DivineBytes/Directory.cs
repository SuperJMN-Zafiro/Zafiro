namespace Zafiro.DivineBytes;

public class Directory : IDirectory
{
    public string Name { get; }
    public IEnumerable<INamed> Children { get; }
    
    // Constructor privado para la implementación interna
    private Directory(string name, IEnumerable<INamed> children)
    {
        Name = name;
        Children = children.ToList();
    }
    
    // Método de fábrica estático que permite sintaxis fluida
    public static Directory Create(string name, params INamed[] contents)
    {
        return new Directory(name, contents);
    }
    
    // Constructor público que permite la sintaxis sugerida
    public Directory(string name, params INamed[] contents)
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