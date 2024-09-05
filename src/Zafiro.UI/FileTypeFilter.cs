namespace Zafiro.UI;

public class FileTypeFilter
{
    public FileTypeFilter(string description, params string[] extensions)
    {
        Description = description;
        Extensions = extensions;
    }

    public string Description { get; }
    public string[] Extensions { get; }
}