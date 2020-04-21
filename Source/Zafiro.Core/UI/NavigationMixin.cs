using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zafiro.Core.UI
{
    public static class NavigationMixin
    {
        public static Task Go<T>(this INavigation navigation, object parameter = null)
        {
            return navigation.Go(typeof(T), parameter);
        }
    }

    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; }
        string PickFile();
    }

    public class FileTypeFilter
    {
        public string Description { get; }
        public string[] Extensions { get; }

        public FileTypeFilter(string description, params string[] extensions)
        {
            Description = description;
            Extensions = extensions;
        }
    }
}