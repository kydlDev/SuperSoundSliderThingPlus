using BepInEx.Configuration;
using UnityEngine;
using KydlCore.Common;
using SuperSoundSliderThingPlus.Configuration;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Infrastructure;

/// <summary>
/// The one place where the player's slider values meet the pure maths: reads the config at call time (PROJECT_RULES 17: no caching, so a
/// change applies at once) and asks <see cref="SoundMixRules"/> for the loudness factor of a category. Inert (factor 1) while the sound
/// feature is not running (mod switched off).
/// </summary>
internal static class SoundLevels
{
    /// <summary>Expected outcome: a factor 0..1 (1 = the sound is left alone). Never throws; a rejected level (NaN) leaves the sound alone.</summary>
    internal static float Factor(SoundCategory category)
    {
        SoundFeature feature = SoundFeature.Instance;
        if (feature == null || !feature.Running || ModSettings.MasterMute == null) { if (RuleLog.IsOn()) ModLog.Every("SoundLevels.Factor:21", 2d, $"SoundLevels.Factor: returns 1f (line 21) - inputs (category={category})"); return 1f; }
        // a group whose [General] Enable switch is off leaves its sounds alone (user 2026-11-17)
        if (!SoundMixRules.GroupOn(ToSlider(category), ModSettings.MenuSoundsOn, ModSettings.SpecialOn)) { if (RuleLog.IsOn()) ModLog.Every("SoundLevels.Factor:23", 2d, $"SoundLevels.Factor: returns 1f (line 23) - inputs (category={category})"); return 1f; }
        // the master mute is the game's own master volume (D-017): it already scales every sound through the mixer, so only the category counts here
        var r = SoundMixRules.Factor(SoundMixRules.MinMute, MuteOf(category));
        if (!r.IsSuccess) ModLog.Warn("SoundLevels: " + category + " factor rejected - " + r.Reason + " (the sound is left alone)");
        return r.IsSuccess ? r.Value : 1f;
    }

    /// <summary>Expected outcome: the factor 0..1 the game's master volume gives a sound that is routed past the mixer (1 while the feature is not running).</summary>
    internal static float UngroupedMasterFactor()
    {
        if (RuleLog.IsOn()) ModLog.Every("SoundLevels.UngroupedMasterFactor", 2d, $"SoundLevels.UngroupedMasterFactor: feature={(SoundFeature.Instance != null)}, running={(SoundFeature.Instance != null && SoundFeature.Instance.Running)}");
        SoundFeature feature = SoundFeature.Instance;
        if (feature == null || !feature.Running) { if (RuleLog.IsOn()) ModLog.Every("SoundLevels.UngroupedMasterFactor:35", 2d, $"SoundLevels.UngroupedMasterFactor: returns 1f (line 35) - when feature == null || !feature.Running: feature={(feature)}"); return 1f; }
        { var returnedAt36 = Mathf.Sqrt(Integrations.Valheim.GameVolume.Master()); if (RuleLog.IsOn()) ModLog.Every("SoundLevels.UngroupedMasterFactor:36", 2d, $"SoundLevels.UngroupedMasterFactor: returns {returnedAt36} (line 36)"); return returnedAt36; }
    }

    /// <summary>The mute level (percent) of one category slider; 0 while the config is not bound yet.</summary>
    internal static float MuteOf(SoundCategory category) => Read(EntryOf(ToSlider(category)));

    /// <summary>The config entry behind a slider (null while the config is not bound).</summary>
    internal static ConfigEntry<float> EntryOf(SoundSlider slider)
    {
        if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf", 2d, $"SoundLevels.EntryOf(slider={slider})");
        switch (slider)
        {
            case SoundSlider.Master: { var returnedAt48 = ModSettings.MasterMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:48", 2d, $"SoundLevels.EntryOf: returns {returnedAt48} (line 48)"); return returnedAt48; }
            case SoundSlider.Weather: { var returnedAt49 = ModSettings.WeatherMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:49", 2d, $"SoundLevels.EntryOf: returns {returnedAt49} (line 49)"); return returnedAt49; }
            case SoundSlider.WolfPup: { var returnedAt50 = ModSettings.WolfPupMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:50", 2d, $"SoundLevels.EntryOf: returns {returnedAt50} (line 50)"); return returnedAt50; }
            case SoundSlider.ShieldGenerator: { var returnedAt51 = ModSettings.ShieldGeneratorMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:51", 2d, $"SoundLevels.EntryOf: returns {returnedAt51} (line 51)"); return returnedAt51; }
            case SoundSlider.ChickenLove: { var returnedAt52 = ModSettings.ChickenLoveMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:52", 2d, $"SoundLevels.EntryOf: returns {returnedAt52} (line 52)"); return returnedAt52; }
            case SoundSlider.Portal: { var returnedAt53 = ModSettings.PortalMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:53", 2d, $"SoundLevels.EntryOf: returns {returnedAt53} (line 53)"); return returnedAt53; }
            case SoundSlider.Music: { var returnedAt54 = ModSettings.MusicMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:54", 2d, $"SoundLevels.EntryOf: returns {returnedAt54} (line 54)"); return returnedAt54; }
            default: { var returnedAt55 = ModSettings.EffectsMute; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.EntryOf:55", 2d, $"SoundLevels.EntryOf: returns {returnedAt55} (line 55)"); return returnedAt55; }
        }
    }

    /// <summary>The mute level (percent) of a slider; 0 while the config is not bound yet.</summary>
    internal static float MuteOf(SoundSlider slider) => Read(EntryOf(slider));

    private static SoundSlider ToSlider(SoundCategory category)
    {
        if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider", 2d, $"SoundLevels.ToSlider(category={category})");
        switch (category)
        {
            case SoundCategory.Weather: { var returnedAt67 = SoundSlider.Weather; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:67", 2d, $"SoundLevels.ToSlider: returns {returnedAt67} (line 67)"); return returnedAt67; }
            case SoundCategory.WolfPup: { var returnedAt68 = SoundSlider.WolfPup; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:68", 2d, $"SoundLevels.ToSlider: returns {returnedAt68} (line 68)"); return returnedAt68; }
            case SoundCategory.ShieldGenerator: { var returnedAt69 = SoundSlider.ShieldGenerator; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:69", 2d, $"SoundLevels.ToSlider: returns {returnedAt69} (line 69)"); return returnedAt69; }
            case SoundCategory.ChickenLove: { var returnedAt70 = SoundSlider.ChickenLove; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:70", 2d, $"SoundLevels.ToSlider: returns {returnedAt70} (line 70)"); return returnedAt70; }
            case SoundCategory.Portal: { var returnedAt71 = SoundSlider.Portal; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:71", 2d, $"SoundLevels.ToSlider: returns {returnedAt71} (line 71)"); return returnedAt71; }
            default: { var returnedAt72 = SoundSlider.Effects; if (RuleLog.IsOn()) ModLog.Every("SoundLevels.ToSlider:72", 2d, $"SoundLevels.ToSlider: returns {returnedAt72} (line 72)"); return returnedAt72; }
        }
    }

    private static float Read(ConfigEntry<float> entry) => entry == null ? SoundMixRules.DefaultMute : entry.Value;
}
