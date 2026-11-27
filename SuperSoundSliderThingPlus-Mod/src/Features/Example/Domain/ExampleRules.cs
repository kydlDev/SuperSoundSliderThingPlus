using KydlCore.Core;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Example.Domain;

/// <summary>
/// EXAMPLE domain rule (template; replace with the mod's real rules). Pure: facts in, decision out - no Unity / Valheim / KydlCore types,
/// no ModLog (the guard test ArchitectureRulesTests checks it), so every branch is unit-tested (tests/Unit/ExampleRulesTests.cs).
/// </summary>
internal static class ExampleRules
{
    internal const float Min = 0f, Max = 10f, Default = 5f;
    internal const string NotANumber = "not a number", Clamped = "clamped into range", InRange = "in range";

    /// Expected outcome: Success with the value inside [Min, Max] (reason says whether it was clamped); Rejected for NaN / infinity.
    internal static OperationResponse<float> Decide(float requested)
    {
        if (RuleLog.IsOn()) RuleLog.Verbose($"ExampleRules.Decide(requested={requested})");
        if (float.IsNaN(requested) || float.IsInfinity(requested)) { var returnedAt19 = OperationResponse<float>.Rejected(NotANumber); if (RuleLog.IsOn()) RuleLog.Verbose($"ExampleRules.Decide: returns {returnedAt19} (line 19)"); return returnedAt19; }
        float value = requested < Min ? Min : requested > Max ? Max : requested;
        { var returnedAt21 = OperationResponse<float>.Success(value, System.Math.Abs(value - requested) < 1e-6f ? InRange : Clamped); if (RuleLog.IsOn()) RuleLog.Verbose($"ExampleRules.Decide: returns {returnedAt21} (line 21)"); return returnedAt21; }
    }
}
