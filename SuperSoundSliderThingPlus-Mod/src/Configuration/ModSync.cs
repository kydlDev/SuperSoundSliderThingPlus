using BepInEx.Configuration;
using KydlCore.Common;
using KydlCore.ServerSync;
using SuperSoundSliderThingPlus.Bootstrap;

namespace SuperSoundSliderThingPlus.Configuration;

/// <summary>
/// Network compatibility and the server's say over its settings, through KydlCore 1.2.0's KyNetwork (ServerSync inside KydlCore; no Jotunn,
/// MIGRATION_PLAN part 0). EveryoneMustHave: every player on a server needs this mod in exactly this version (the old EveryoneMustHaveMod + Patch);
/// a client-only mod uses KyNetworkLevel.ClientOnly (a player with it still joins servers without it; equal versions when both have it).
/// A shared setting is locked for players who are not server admins; the server's value is used.
/// </summary>
internal static class ModSync
{
    /// <summary>The mod's ServerSync instance (made by KydlCore on first use, which is in ModSettings.Bind / Plugin.Awake).</summary>
    public static readonly ConfigSync Sync = KyNetwork.Register(Plugin.ModGuid, Plugin.ModName, Plugin.ModVersion, KyNetworkLevel.ClientOnly);

    /// <summary>Starts the version check (the first use of this class does it). Called once in Plugin.Awake.</summary>
    public static void Start()
    {
        ModLog.Verbose($"ModSync.Start: network check active for {Sync.Name} {Sync.CurrentVersion} (required on every player: {Sync.ModRequired})");
    }

    /// <summary>Expected outcome: the entry is decided by the server (returns that entry).</summary>
    public static ConfigEntry<T> Share<T>(ConfigEntry<T> entry)
    {
        var shared = KyNetwork.Share(Sync, entry);
        ModLog.Verbose($"ModSync.Share: [{entry.Definition.Section}] {entry.Definition.Key} is decided by the server (mod version {Plugin.ModVersion})");
        return shared;
    }

    /// <summary>Every entry tagged ConfigurationManagerAttributes.IsAdminOnly = true is decided by the server. Call after the last Bind. Expected outcome: the number shared.</summary>
    public static int ShareAdminOnly(ConfigFile config)
    {
        int shared = KyNetwork.ShareAdminOnly(Sync, config);
        ModLog.Verbose($"ModSync.ShareAdminOnly: {shared} admin-only setting(s) decided by the server");
        return shared;
    }
}
