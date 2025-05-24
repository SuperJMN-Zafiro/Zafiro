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
            var viewModelTypes = assembly.GetTypes().Where(Extensions.IsSection);

            foreach (var viewModelType in viewModelTypes)
            {
                string sectionName = viewModelType.Name.Replace("ViewModel", "");
                string formattedName = string.Concat(sectionName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                var method = typeof(SectionsBuilder).GetMethod("Add")?.MakeGenericMethod(viewModelType);
                var iconId = viewModelType.GetCustomAttribute<SectionAttribute>()!.Id;
                method?.Invoke(builder, new object[] { formattedName, new Icon { IconId = iconId }, true });
            }
        });

        return services;
    }
}