using System.Runtime.CompilerServices;
using HarmonyLib;
using KydlCore.Common;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Features.Sound.Infrastructure;
using SuperSoundSliderThingPlus.Integrations.Valheim;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Integrations.Harmony;

/// <summary>
/// Scales every sound effect by its category. ZSFX.CustomUpdate sets the AudioSource volume to an absolute value on every call
/// (verified in the IL, 2026-11-10), so multiplying afterwards cannot compound. A sound is classified once, the first time it is seen,
/// and recorded for the in-game tests. Inert while the sound feature is off (SoundLevels returns 1).
/// </summary>
[HarmonyPatch(typeof(ZSFX), nameof(ZSFX.CustomUpdate))]
internal static class ZsfxCustomUpdatePatch
{
    private sealed class SoundEntry
    {
        internal SoundCategory Category;
        internal float Raw, Scaled;   // the volume the game set / the volume after the mod scaled it (tests read them)
        internal float LastScale = 1f;   // what the volume was multiplied by on the previous call (the heard audio was made with it)
    }

    private static readonly ConditionalWeakTable<ZSFX, SoundEntry> Entries = new ConditionalWeakTable<ZSFX, SoundEntry>();
    private static readonly ConditionalWeakTable<ZSFX, SoundEntry>.CreateValueCallback Classify = ClassifyNew;

    [HarmonyPostfix]
    private static void Postfix(ZSFX __instance, AudioSource ___m_audioSource)
    {
        if (RuleLog.IsOn()) ModLog.Every("SoundEntry.Postfix", 2d, $"SoundEntry.Postfix(__instance={__instance}, ___m_audioSource={___m_audioSource})");
        if (___m_audioSource == null) { if (RuleLog.IsOn()) ModLog.Every("SoundEntry.Postfix:34", 2d, $"SoundEntry.Postfix: returns (line 34) - inputs (__instance={__instance}, ___m_audioSource={___m_audioSource})"); return; }
        SoundEntry entry = Entries.GetValue(__instance, Classify);
        entry.Raw = ___m_audioSource.volume;
        // the game has just set the volume (unscaled); the audio heard right now was made with the previous scale applied
        LoudnessEqualizer.Observe(___m_audioSource, ___m_audioSource.volume * entry.LastScale);
        float scale = SoundLevels.Factor(entry.Category) * LoudnessEqualizer.GainFor(___m_audioSource.clip);
        // a source without a mixer group (the candle sounds) never meets the game's master volume: apply it here, with the mixer's curve (amplitude = sqrt(volume))
        if (___m_audioSource.outputAudioMixerGroup == null) scale *= SoundLevels.UngroupedMasterFactor();
        entry.LastScale = scale;
        if (!FloatCompare.Same(scale, 1f)) ___m_audioSource.volume *= scale;
        entry.Scaled = ___m_audioSource.volume;
    }

    /// <summary>Tests: what the patch last saw and did for one sound. Expected outcome: true when the patch has handled the sound at least once.</summary>
    internal static bool TryGetLast(ZSFX sfx, out SoundCategory category, out float raw, out float scaled)
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"SoundEntry.TryGetLast(sfx={sfx})");
        SoundEntry entry;
        category = SoundCategory.Effects;
        raw = scaled = 0f;
        if (sfx == null || !Entries.TryGetValue(sfx, out entry)) { if (RuleLog.IsOn()) ModLog.Verbose($"SoundEntry.TryGetLast: returns false (line 54) - inputs (sfx={sfx})"); return false; }
        category = entry.Category;
        raw = entry.Raw;
        scaled = entry.Scaled;
        { if (RuleLog.IsOn()) ModLog.Verbose($"SoundEntry.TryGetLast: returns true (line 58) - inputs (sfx={sfx})"); return true; }
    }

    private static SoundEntry ClassifyNew(ZSFX sfx)
    {
        string sound = sfx.gameObject.name;
        string owner = SoundOwner.NameFor(sfx);
        SoundCategory category = SoundClassifier.Classify(sound, owner);
        bool first = SoundRecorder.Record(sound, owner, category);
        if (first) ModLog.Verbose("ZsfxCustomUpdatePatch: new sound '" + sound + "' owner '" + owner + "' -> " + category);
        return new SoundEntry { Category = category };
    }
}
