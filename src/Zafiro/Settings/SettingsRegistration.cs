using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Zafiro.Settings;

public static class SettingsRegistration
{
    public static IServiceCollection AddSettings<T>(
        this IServiceCollection services,
        string company,
        string product,
        string fileName,
        Func<T> createDefault)
    {
        services.AddSingleton<ISettingsStore, JsonSettingsStore>();

        services.AddSingleton<ISettings<T>>(sp =>
        {
            var store = sp.GetRequiredService<ISettingsStore>();
            var path = Path.Combine(GetSettingsDir(company, product), fileName);
            return new JsonSettings<T>(path, store, createDefault);
        });

        return services;
    }

    private static string GetSettingsDir(string company, string product)
    {
        var baseDir = OperatingSystem.IsWindows() ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.ApplicationData;
        var root = Environment.GetFolderPath(baseDir);
        var dir = Path.Combine(root, company, product);
        Directory.CreateDirectory(dir);
        return dir;
    }
}