using DiscordRPC;
using Godot;

public class Discord
{
    private static readonly string appId = "1231699688340590722";
    public static DiscordRpcClient Client { get; private set; }
    public static bool Enabled => PlatformCapabilities.SupportsDiscordRichPresence && Client != null;

    static Discord()
    {
        if (!PlatformCapabilities.SupportsDiscordRichPresence)
        {
            return;
        }

        try
        {
            Client = new DiscordRpcClient(appId)
            {
            };

            Client.Initialize();
            Client.SetPresence(new RichPresence()
            {
                Assets = new Assets()
                {
                    LargeImageKey = "short"
                },
            });
        }
        catch (System.Exception exception)
        {
            Client = null;
            GD.PrintErr($"Discord RPC unavailable: {exception.Message}");
        }
    }

    public static void UpdateDetails(string details)
    {
        if (!Enabled)
        {
            return;
        }

        Client.UpdateDetails(details);
    }

    public static void UpdateState(string state)
    {
        if (!Enabled)
        {
            return;
        }

        Client.UpdateState(state);
    }

    public static void DisposeClient()
    {
        if (!Enabled)
        {
            return;
        }

        Client.Dispose();
        Client = null;
    }
}
