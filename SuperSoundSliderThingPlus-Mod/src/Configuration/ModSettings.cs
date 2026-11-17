using BepInEx.Configuration;
using KydlCore.Common;

namespace SuperSoundSliderThingPlus.Configuration;

/// <summary>
/// All settings in one place (user rule, every mod):
///  * PLAYER CONFIG (BepInEx entries below, bound once from Plugin.Awake): only what a player should change. Clear descriptions, ranges,
///    drop-downs (enums). Descriptions never mention older versions, old names or old values.
///  * DEV-ONLY values (Tunable fields below): fixed values chosen by the user, NOT in the player's file. The user's KydlTools mod (F7) finds
///    every static Tunable here by itself and lets the user change it in game - nothing to register.
/// When the user settles a value in game, it becomes the new fixed value / default here.
/// </summary>
internal static class ModSettings
{
    // [General]
    public static ConfigEntry<bool> EnableMod;
    // no MenuKey: the mod's menu is a set of tabs in the KY hub (F6, KydlCore), see src/Presentation/Hub/Registration/HubTabs.cs
    // One functionality = one [General] switch "Enable <Category>" (PROJECT_RULES 11), only when the mod has several of them, e.g.:
    //   public static ConfigEntry<bool> ColoursEnable;   // bound in General as "Enable Colours" with the value of the old Colours Enable entry;  on only when On && ColoursEnable.Value
    // the sliders (D-004, D-011): every one is a MUTE level in percent, 0 = full loudness, 100 = muted; Master, Music and Sound Effects live in
    // [Game menu sounds], the others in [Special adjustments] (user 2026-11-17)
    public static ConfigEntry<float> MasterMute, EffectsMute, WeatherMute, WolfPupMute, ShieldGeneratorMute, ChickenLoveMute, PortalMute, MusicMute;
    // [General] matching (D-005 step 2): off by default, the player switches it on
    public static ConfigEntry<bool> EqualizeLoudness, DefaultsApplied;
    // [General] one switch per group of sliders (PROJECT_RULES 11); the groups are the sections below (user 2026-11-17)
    public const string SectionMenuSounds = "Game menu sounds", SectionSpecial = "Special adjustments";
    public static ConfigEntry<bool> MenuSoundsEnable, SpecialEnable;
    // [DebugLog]
    public static ConfigEntry<bool> DebugLog;

    /// Is the mod switched on ([General] Enable Mod)? Off = inert: no effects; nothing built is removed.
    public static bool On { get { return EnableMod == null || EnableMod.Value; } }

    /// Do the [Game menu sounds] sliders (Master, Music, Sound Effects) work? Enable Mod AND Enable Game Menu Sounds; read at call time.
    public static bool MenuSoundsOn { get { return On && (MenuSoundsEnable == null || MenuSoundsEnable.Value); } }

    /// Do the [Special adjustments] sliders (Portal, Shield Generator, Weather, Wolf Pup, Chicken Love) work? Enable Mod AND Enable Special Adjustments.
    public static bool SpecialOn { get { return On && (SpecialEnable == null || SpecialEnable.Value); } }

    /// The gate of one feature (IFeature.IsEnabled): Enable Mod AND its own [General] Enable <Feature> (null = a mod with one feature: Enable Mod only).
    /// Read at call time, never cached, so a config change applies at once.
    public static bool FeatureOn(ConfigEntry<bool> ownEnable) { return On && (ownEnable == null || ownEnable.Value); }

    // --- dev-only fixed values (Tunables): not in the config file, KydlTools changes them for testing -----------------------------------
    public static readonly Tunable<float> ExampleSpeed = new Tunable<float>("Example", "ExampleSpeed", 1f, 0f, 10f,
        "Example dev-only value (0-10, with a range as PROJECT_RULES 11 asks): replace or remove. Read it with ModSettings.ExampleSpeed.Value.");

    public static readonly Tunable<float> OwnerRadius = new Tunable<float>("Sound", "OwnerRadius", 1.5f, 0.1f, 5f,
        "Dev-only (metres, 0.1-5): a sound belongs to the creature standing within this distance of it (tells a wolf pup's sounds from an adult's).");

    public static readonly Tunable<float> TargetRms = new Tunable<float>("Loudness", "TargetRms", 0.1f, 0.01f, 0.5f,
        "Dev-only (RMS 0.01-0.5): the loudness every clip is scaled toward while Equalize is on.");
    public static readonly Tunable<float> MinGain = new Tunable<float>("Loudness", "MinGain", 0.25f, 0.05f, 1f,
        "Dev-only (0.05-1): equalizing never cuts a clip below this factor.");
    public static readonly Tunable<float> MaxGain = new Tunable<float>("Loudness", "MaxGain", 3f, 1f, 8f,
        "Dev-only (1-8): equalizing never boosts a clip above this factor (a very quiet clip stays quiet instead of turning into noise).");
    public static readonly Tunable<float> MeasureSeconds = new Tunable<float>("Loudness", "MeasureSeconds", 1f, 0.1f, 5f,
        "Dev-only (seconds 0.1-5): the length of the window from the middle of a clip that is measured.");

/// The value the player had in an entry that moved (still in the file as an orphan before DropRemoved deletes it): the first of the
/// given (section, key, section, key, ...) pairs that holds a bool; true when there is none.
private static bool Old(ConfigFile c, params string[] sectionAndKey)
{
    // OrphanedEntries is not a public member of every BepInEx 5 build (ConfigTools.DropRemoved reaches it the same way)
    System.Reflection.PropertyInfo orphans = typeof(ConfigFile).GetProperty("OrphanedEntries", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
    var dict = orphans != null ? orphans.GetValue(c, null) as System.Collections.IDictionary : null;
    for (int i = 0; dict != null && i + 1 < sectionAndKey.Length; i += 2)
    {
        var def = new ConfigDefinition(sectionAndKey[i], sectionAndKey[i + 1]);
        bool b;
        if (dict.Contains(def) && bool.TryParse((dict[def] as string ?? "").Trim(), out b))
        {
            return b;
        }
    }

    return true;
}

    /// Expected outcome: every player entry is bound (returns that check).
    public static bool Bind(ConfigFile c)
    {
        var savedLines = ConfigTools.Capture(c);     // before the first Bind: every Bind saves the file with what BepInEx parsed

        EnableMod = c.Bind("General", "Enable Mod", Old(c, "General", "Enable mod", "General", "EnableAll"),
            ConfigOrder.First("Switches the whole mod on / off while playing. Off: no effects from this mod; things already built stay in the world."));

        // one functionality = one [General] switch "Enable <Category>" (PROJECT_RULES 11); each group of sliders has its own section (user 2026-11-17)
        MenuSoundsEnable = c.Bind("General", "Enable Game Menu Sounds", true,
            "The sliders of [Game menu sounds] (Master, Music, Sound Effects mute) work. Off = they do nothing and the game's own volumes stay as they are.");
        SpecialEnable = c.Bind("General", "Enable Special Adjustments", true,
            "The sliders of [Special adjustments] (Portal, Shield Generator, Weather, Wolf Pup, Chicken Love mute) work. Off = those sounds are left alone.");
        EqualizeLoudness = c.Bind("General", "EqualizeLoudness", true, "Match the loudness of sounds: every sound clip is measured once and scaled toward one target loudness, so loud sounds come down and quiet ones come up. Off = the game's own balance.");
        DefaultsApplied = c.Bind("General", "DefaultsApplied", false, "Set to true once the first world entry has set the category sliders to 50%. Set it to false to apply those starting values once more.");

        MasterMute = BindMute(c, SectionMenuSounds, "Master mute", "MasterMute", "Master mute level in percent: 0 = every sound at full loudness, 100 = everything muted. Scales all the sliders.");
        MusicMute = BindMute(c, SectionMenuSounds, "Music mute", "MusicMute", "Mute level in percent for the music (the game's own music slider): 0 = full loudness, 100 = muted.");
        EffectsMute = BindMute(c, SectionMenuSounds, "Sound Effects mute", "SoundEffectsMute", "Mute level in percent for all sound effects that have no slider of their own (the game's own sound effects slider follows it): 0 = full loudness, 100 = muted.");

        PortalMute = BindMute(c, SectionSpecial, "Portal mute", "PortalMute", "Mute level in percent for portal sounds: 0 = full loudness, 100 = muted.");
        ShieldGeneratorMute = BindMute(c, SectionSpecial, "Shield Generator mute", "ShieldGeneratorMute", "Mute level in percent for shield generator sounds: 0 = full loudness, 100 = muted.");
        WeatherMute = BindMute(c, SectionSpecial, "Weather mute", "WeatherMute", "Mute level in percent for world weather sounds (rain, wind, thunder): 0 = full loudness, 100 = muted.");
        WolfPupMute = BindMute(c, SectionSpecial, "Wolf Pup mute", "WolfPupMute", "Mute level in percent for wolf pup sounds: 0 = full loudness, 100 = muted.");
        ChickenLoveMute = BindMute(c, SectionSpecial, "Chicken Love mute", "ChickenLoveMute", "Mute level in percent for the chicken love sound: 0 = full loudness, 100 = muted.");

        DebugLog = c.Bind("DebugLog", "Enable", false,
            "Writes detailed lines about everything the mod does to the BepInEx log (for bug reports). Leave off while playing normally.");

        ConfigTools.ApplyCommaFix(c, savedLines);    // after every Bind: "3,5" typed by hand means 3.5, not 35
        ConfigTools.DropRemoved(c);

        EnableMod.SettingChanged += (s, e) => ModLog.Info("Config: Enable Mod = " + EnableMod.Value + (EnableMod.Value ? ", the mod is on" : ", the mod is inert (no effects)"));
        ModLog.Verbose("Config: bound (Enable Mod " + EnableMod.Value + ", debug log " + DebugLog.Value + ")");
        return EnableMod != null && DebugLog != null && MasterMute != null && ChickenLoveMute != null && DefaultsApplied != null;
    }

    /// The mute level (0..100 %) of one slider in its own section; the level the player had under the old [General] key is carried over.
    private static ConfigEntry<float> BindMute(ConfigFile c, string section, string key, string oldKey, string description)
        => c.Bind(section, key, OldFloat(c, 0f, "General", oldKey), new ConfigDescription(description, new AcceptableValueRange<float>(0f, 100f)));

    /// The number the player had in an entry that moved or got another name (still in the file as an orphan before DropRemoved deletes it): the first
    /// of the given (section, key, section, key, ...) pairs that holds a number inside 0..100; the fallback when there is none.
    private static float OldFloat(ConfigFile c, float fallback, params string[] sectionAndKey)
    {
        System.Reflection.PropertyInfo orphans = typeof(ConfigFile).GetProperty("OrphanedEntries", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        var dict = orphans != null ? orphans.GetValue(c, null) as System.Collections.IDictionary : null;
        for (int i = 0; dict != null && i + 1 < sectionAndKey.Length; i += 2)
        {
            var def = new ConfigDefinition(sectionAndKey[i], sectionAndKey[i + 1]);
            float f;
            if (dict.Contains(def) && float.TryParse((dict[def] as string ?? "").Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out f) && f >= 0f && f <= 100f)
            {
                return f;
            }
        }

        return fallback;
    }
}
