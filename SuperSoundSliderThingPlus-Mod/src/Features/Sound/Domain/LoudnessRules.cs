using KydlCore.Core;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Domain;

/// <summary>
/// Pure loudness-matching maths (D-005 step 2): the loudness of a clip is the RMS of a window of its samples, and the gain that
/// brings it to the target is target / measured, kept inside [min, max] so a very quiet clip is never boosted into noise and a loud
/// one is never cut to nothing. No Unity / Valheim types and no ModLog (guard test): every branch is unit-tested.
/// </summary>
internal static class LoudnessRules
{
    internal const string NotMeasurable = "no usable measurement", BadLimits = "gain limits are not valid", Limited = "limited to the gain range", Exact = "exact";
    internal const float SilenceBelow = 0.0001f;

    /// <summary>Expected outcome: Success with the RMS (0..1+) of the first `count` samples; Rejected for no samples or a null array.</summary>
    internal static OperationResponse<float> Rms(float[] samples, int count)
    {
        if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Rms", 2d, $"LoudnessRules.Rms(samples={samples}, count={count})");
        // Stryker disable once Equality : equivalent - count 0 gives 0 / 0 = NaN, which is rejected with the same reason a few lines below
        if (samples == null || count <= 0 || count > samples.Length) { var returnedAt21 = OperationResponse<float>.Rejected(NotMeasurable); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Rms:21", 2d, $"LoudnessRules.Rms: returns {returnedAt21} (line 21)"); return returnedAt21; }
        double sum = 0;
        for (int i = 0; i < count; i++)
        {
            double s = samples[i];
            sum += s * s;
        }
        double rms = System.Math.Sqrt(sum / count);
        if (double.IsNaN(rms) || double.IsInfinity(rms)) { var returnedAt29 = OperationResponse<float>.Rejected(NotMeasurable); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Rms:29", 2d, $"LoudnessRules.Rms: returns {returnedAt29} (line 29)"); return returnedAt29; }
        { var returnedAt30 = OperationResponse<float>.Success((float)rms, Exact); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Rms:30", 2d, $"LoudnessRules.Rms: returns {returnedAt30} (line 30)"); return returnedAt30; }
    }

    /// <summary>
    /// Expected outcome: Success with the gain (reason says whether the range limited it); a silent or invalid measurement gives Success 1
    /// (the clip is left alone) - never a huge boost; Rejected when the target or the limits are not valid numbers (min &gt; max, min &lt;= 0).
    /// </summary>
    internal static OperationResponse<float> Gain(float measuredRms, float targetRms, float minGain, float maxGain)
    {
        if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Gain", 2d, $"LoudnessRules.Gain(measuredRms={measuredRms}, targetRms={targetRms}, minGain={minGain}, maxGain={maxGain})");
        if (!IsNumber(targetRms) || targetRms <= 0f || !IsNumber(minGain) || !IsNumber(maxGain) || minGain <= 0f || minGain > maxGain)
            { var returnedAt41 = OperationResponse<float>.Rejected(BadLimits); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Gain:41", 2d, $"LoudnessRules.Gain: returns {returnedAt41} (line 41)"); return returnedAt41; }
        if (!IsNumber(measuredRms) || measuredRms < SilenceBelow) { var returnedAt42 = OperationResponse<float>.Success(1f, NotMeasurable); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Gain:42", 2d, $"LoudnessRules.Gain: returns {returnedAt42} (line 42)"); return returnedAt42; }
        float gain = targetRms / measuredRms;
        // Stryker disable once Equality : equivalent - at the limit itself both comparisons give the same value
        float limited = gain < minGain ? minGain : gain > maxGain ? maxGain : gain;
        { var returnedAt46 = OperationResponse<float>.Success(limited, FloatCompare.Same(limited, gain) ? Exact : Limited); if (RuleLog.IsOn()) RuleLog.Every("LoudnessRules.Gain:46", 2d, $"LoudnessRules.Gain: returns {returnedAt46} (line 46)"); return returnedAt46; }
    }

    private static bool IsNumber(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
}
