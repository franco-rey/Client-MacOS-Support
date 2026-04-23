using Godot;

public static class PlatformCapabilities
{
    private static readonly string PlatformName = OS.GetName();

    internal static bool IsMacOS => PlatformName == "macOS";

    public static bool SupportsBackgroundVideo => !IsMacOS;

    public static bool SupportsSelfUpdateInstall => !IsMacOS;

    public static bool SupportsDiscordRichPresence => !IsMacOS;
}
