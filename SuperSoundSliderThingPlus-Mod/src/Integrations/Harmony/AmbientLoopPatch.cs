using HarmonyLib;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Features.Sound.Infrastructure;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;
using KydlCore.Common;

namespace SuperSoundSliderThingPlus.Integrations.Harmony;

/// <summary>
/// The environment's ambient loop (rain, storm ...) belongs to the weather slider. AudioMan.UpdateAmbientLoop moves the volume in steps
/// towards m_ambientVol (MoveTowards), so multiplying would compound: the volume is clamped to a ceiling of m_ambientVol x factor instead.
/// </summary>
[HarmonyPatch(typeof(AudioMan), "UpdateAmbientLoop")]
internal static class AmbientLoopPatch
{
    [HarmonyPostfix]
    private static void Postfix(AudioSource ___m_ambientLoopSource, float ___m_ambientVol)
    {
        if (RuleLog.IsOn()) ModLog.Every("AmbientLoopPatch.Postfix", 2d, $"AmbientLoopPatch.Postfix(___m_ambientLoopSource={___m_ambientLoopSource}, ___m_ambientVol={___m_ambientVol})");
        if (___m_ambientLoopSource == null) { if (RuleLog.IsOn()) ModLog.Every("AmbientLoopPatch.Postfix:21", 2d, $"AmbientLoopPatch.Postfix: returns (line 21) - inputs (___m_ambientLoopSource={___m_ambientLoopSource}, ___m_ambientVol={___m_ambientVol})"); return; }
        float factor = SoundLevels.Factor(SoundCategory.Weather);
        if (factor >= 1f) { if (RuleLog.IsOn()) ModLog.Every("AmbientLoopPatch.Postfix:23", 2d, $"AmbientLoopPatch.Postfix: returns (line 23) - inputs (___m_ambientLoopSource={___m_ambientLoopSource}, ___m_ambientVol={___m_ambientVol})"); return; }
        float ceiling = ___m_ambientVol * factor;
        if (___m_ambientLoopSource.volume > ceiling) ___m_ambientLoopSource.volume = ceiling;
    }
}
