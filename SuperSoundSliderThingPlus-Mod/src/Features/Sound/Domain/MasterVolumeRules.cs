using KydlCore.Core;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Domain;

/// <summary>
/// Pure maths between the game's master volume (0 = silent, 1 = full, the player's in-game slider) and this mod's master MUTE level
/// (0 = full loudness, 100 = muted): they are the same control seen from two sides, so mute = (1 - volume) x 100 (D-017).
/// No Unity / Valheim types and no ModLog (guard test).
/// </summary>
internal static class MasterVolumeRules
{
    internal const float SameWithin = 0.05f;
    internal const string NotANumber = "not a number", Clamped = "clamped into range", InRange = "in range";

    /// <summary>Expected outcome: Success with the mute level 0..100 (one decimal) for a game volume; the volume is clamped into 0..1; Rejected for NaN / infinity.</summary>
    internal static OperationResponse<float> MuteFromVolume(float volume)
    {
        if (RuleLog.IsOn()) RuleLog.Every("MasterVolumeRules.MuteFromVolume", 2d, $"MasterVolumeRules.MuteFromVolume(volume={volume})");
        if (!IsNumber(volume)) { var returnedAt20 = OperationResponse<float>.Rejected(NotANumber); if (RuleLog.IsOn()) RuleLog.Every("MasterVolumeRules.MuteFromVolume:20", 2d, $"MasterVolumeRules.MuteFromVolume: returns {returnedAt20} (line 20)"); return returnedAt20; }
        // Stryker disable once Equality : equivalent - at the limit itself both comparisons give the same value
        float v = volume < 0f ? 0f : volume > 1f ? 1f : volume;
        float mute = (float)System.Math.Round((1f - v) * 100f, 1);
        { var returnedAt24 = OperationResponse<float>.Success(mute, FloatCompare.Same(v, volume) ? InRange : Clamped); if (RuleLog.IsOn()) RuleLog.Every("MasterVolumeRules.MuteFromVolume:24", 2d, $"MasterVolumeRules.MuteFromVolume: returns {returnedAt24} (line 24)"); return returnedAt24; }
    }

    /// <summary>Expected outcome: Success with the game volume 0..1 for a mute level; the level is clamped into 0..100; Rejected for NaN / infinity.</summary>
    internal static OperationResponse<float> VolumeFromMute(float mute)
    {
        if (RuleLog.IsOn()) RuleLog.Verbose($"MasterVolumeRules.VolumeFromMute(mute={mute})");
        if (!IsNumber(mute)) { var returnedAt31 = OperationResponse<float>.Rejected(NotANumber); if (RuleLog.IsOn()) RuleLog.Verbose($"MasterVolumeRules.VolumeFromMute: returns {returnedAt31} (line 31)"); return returnedAt31; }
        // Stryker disable once Equality : equivalent - at the limit itself both comparisons give the same value
        float m = mute < 0f ? 0f : mute > 100f ? 100f : mute;
        { var returnedAt34 = OperationResponse<float>.Success(1f - m / 100f, FloatCompare.Same(m, mute) ? InRange : Clamped); if (RuleLog.IsOn()) RuleLog.Verbose($"MasterVolumeRules.VolumeFromMute: returns {returnedAt34} (line 34)"); return returnedAt34; }
    }

    /// <summary>True when two mute levels are the same slider position (rounding noise of the float round trip is ignored).</summary>
    internal static bool SameMute(float a, float b) => System.Math.Abs(a - b) < SameWithin;

    private static bool IsNumber(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}
