using System;

namespace Zafiro.Settings;

public sealed record AppSettings(
    string Theme,
    bool UseHardwareAcceleration,
    string[] RecentFiles)
{
    public static AppSettings Default() => new(
        Theme: "Light",
        UseHardwareAcceleration: true,
        RecentFiles: Array.Empty<string>());
}