using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.UI.Navigation;

namespace Zafiro.UI.Shell.Utils;

public static class NavigationExtensions
{
    public static IServiceCollection AddAllSections(this IServiceCollection services, Assembly assembly)
    {
        services.RegisterSections(builder =>
        {
            var sections = from sectionType in assembly.GetTypes().Where(Extensions.IsSection)
                let sectionAttribute = sectionType.GetCustomAttribute<SectionAttribute>()
                select new { sectionType = sectionType, sectionAttribute };

            foreach (var viewModelType in sections.OrderBy(arg => arg.sectionAttribute.SortIndex))
            {
                var type = viewModelType.sectionType;
                var icon = viewModelType.sectionAttribute.Icon;

                string sectionName = type.Name.Replace("ViewModel", "");
                string formattedName = string.Concat(sectionName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                var method = typeof(SectionsBuilder).GetMethod("Add")?.MakeGenericMethod(type);
                method?.Invoke(builder, new object[] { formattedName, new Icon { Source = icon ?? "fa-window-maximize" }, true });
            }
        });

        return services;
    }
}