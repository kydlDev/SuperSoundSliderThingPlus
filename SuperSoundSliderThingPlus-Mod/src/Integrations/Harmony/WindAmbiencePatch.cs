using HarmonyLib;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Features.Sound.Infrastructure;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;
using KydlCore.Common;

namespace SuperSoundSliderThingPlus.Integrations.Harmony;

/// <summary>
/// The wind loop belongs to the weather slider. AudioMan.UpdateWindAmbience sets the loop's volume from the wind intensity (an absolute
/// Lerp, verified in the IL), so multiplying afterwards cannot compound.
/// </summary>
[HarmonyPatch(typeof(AudioMan), "UpdateWindAmbience")]
internal static class WindAmbiencePatch
{
    [HarmonyPostfix]
    private static void Postfix(AudioSource ___m_windLoopSource)
    {
        if (RuleLog.IsOn()) ModLog.Every("WindAmbiencePatch.Postfix", 2d, $"WindAmbiencePatch.Postfix(___m_windLoopSource={___m_windLoopSource})");
        if (___m_windLoopSource == null) { if (RuleLog.IsOn()) ModLog.Every("WindAmbiencePatch.Postfix:21", 2d, $"WindAmbiencePatch.Postfix: returns (line 21) - inputs (___m_windLoopSource={___m_windLoopSource})"); return; }
        float factor = SoundLevels.Factor(SoundCategory.Weather);
        if (factor < 1f) ___m_windLoopSource.volume *= factor;
    }
}
