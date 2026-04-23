using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Octokit;
using Updatum;

public class Releases
{
    private const string LatestReleaseUrl = "https://github.com/Rhythia/Client/releases/latest";
    private static bool initialized = false;

    public static bool SupportsSelfUpdateInstall => PlatformCapabilities.SupportsSelfUpdateInstall;
    public static Version GetCurrentVersion => Version.Parse((string)ProjectSettings.GetSetting("application/config/version"));

    private static string GetAssetRegexPattern()
    {
        return OS.GetName() switch
        {
            "Windows" => "(?i)(windows|win)",
            "Linux" => "(?i)(linux)",
            "macOS" => "(?i)(macos|osx|darwin|mac)",
            _ => $"(?i)({OS.GetName()})",
        };
    }

    public static readonly UpdatumManager MANAGER = new("Rhythia", "Client", currentVersion: GetCurrentVersion)
    {
        AssetRegexPattern = GetAssetRegexPattern(),
    };

    // TODO: Add update request in menus when updates are found
    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        MANAGER.AutoUpdateCheckTimer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
        initialized = true;
    }

    public static async Task<bool> CheckForUpdatesAsync()
    {
        Initialize();
        return await MANAGER.CheckForUpdatesAsync();
    }

    public static async void UpdateToLatest()
    {
        if (!SupportsSelfUpdateInstall)
        {
            OpenLatestReleasePage();
            return;
        }

        var release = await DownloadReleaseAsync();

        await InstallUpdateAsync(release);
    }

    public static async Task<UpdatumDownloadedAsset> DownloadReleaseAsync(Release release = null)
    {
        release ??= MANAGER.LatestRelease;

        if (release == null)
        {
            return null;
        }

        var asset = await MANAGER.DownloadUpdateAsync(release);
        return asset;
    }

    public static async Task<bool> InstallUpdateAsync(UpdatumDownloadedAsset asset)
    {
        if (!SupportsSelfUpdateInstall)
        {
            Logger.Log("Self-updating installs are unavailable on this platform.");
            return false;
        }

        if (OS.HasFeature("editor") || OS.HasFeature("debug"))
        {
            Logger.Error("Can not install update while in editor/debug");
            return false;
        }

        return await MANAGER.InstallUpdateAsync(asset);
    }

    public static void OpenLatestReleasePage()
    {
        string url = MANAGER.LatestRelease?.HtmlUrl ?? LatestReleaseUrl;
        OS.ShellOpen(url);
    }
}
