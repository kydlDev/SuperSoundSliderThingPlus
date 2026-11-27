using KydlCore.Common;
using SuperSoundSliderThingPlus.Configuration;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Integrations.Valheim;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Infrastructure;

/// <summary>
/// The loudness-matching switch board (D-005 step 2, D-015): while the feature runs and "EqualizeLoudness" is on, each clip's level is
/// learned while it plays and the clip is scaled toward the target. Read at call time (PROJECT_RULES 17), so the toggle and the dev tunables apply at once.
/// A clip that has not been heard long enough yet plays at the game's own level. Only ZSFX sounds are matched; the ambient / wind / shield-hum
/// sources are single looping sources the game balances itself.
/// </summary>
internal static class LoudnessEqualizer
{
    /// <summary>True while matching is switched on and the feature runs.</summary>
    internal static bool Active()
    {
        if (RuleLog.IsOn()) ModLog.Every("LoudnessEqualizer.Active", 2d, $"LoudnessEqualizer.Active: feature={(SoundFeature.Instance != null)}, running={(SoundFeature.Instance != null && SoundFeature.Instance.Running)}, setting={(ModSettings.EqualizeLoudness != null && ModSettings.EqualizeLoudness.Value)}");
        SoundFeature feature = SoundFeature.Instance;
        { var returnedAt23 = feature != null && feature.Running && ModSettings.EqualizeLoudness != null && ModSettings.EqualizeLoudness.Value; if (RuleLog.IsOn()) ModLog.Every("LoudnessEqualizer.Active:23", 2d, $"LoudnessEqualizer.Active: returns {returnedAt23} (line 23)"); return returnedAt23; }
    }

    /// <summary>Expected outcome: a gain (1 = leave alone). Never throws; anything unusable (off, no clip, not learned yet, bad limits) is 1.</summary>
    internal static float GainFor(AudioClip clip)
    {
        if (!Active()) { if (RuleLog.IsOn()) ModLog.Every("LoudnessEqualizer.GainFor:29", 2d, $"LoudnessEqualizer.GainFor: returns 1f (line 29) - inputs (clip={clip})"); return 1f; }
        float rms = ClipMeter.RmsOf(clip);
        if (rms <= ClipMeter.Unknown) { if (RuleLog.IsOn()) ModLog.Every("LoudnessEqualizer.GainFor:31", 2d, $"LoudnessEqualizer.GainFor: returns 1f (line 31) - inputs (clip={clip})"); return 1f; }
        var r = LoudnessRules.Gain(rms, ModSettings.TargetRms.Value, ModSettings.MinGain.Value, ModSettings.MaxGain.Value);
        if (!r.IsSuccess)
        {
            ModLog.Warn("LoudnessEqualizer: gain rejected - " + r.Reason + " (the clip is left alone)");
            return 1f;
        }
        { var returnedAt38 = r.Value; if (RuleLog.IsOn()) ModLog.Every("LoudnessEqualizer.GainFor:38", 2d, $"LoudnessEqualizer.GainFor: returns {returnedAt38} (line 38)"); return returnedAt38; }
    }

    /// <summary>Lets the meter hear one block of a playing source. `playedVolume` is the volume the source had when the heard block was made.</summary>
    internal static bool Observe(AudioSource source, float playedVolume) => Active() && ClipMeter.Observe(source, playedVolume);
}
