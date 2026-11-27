using System;
using KydlCore.Core;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Execution;

/// <summary>
/// Checks ownership FIRST, then runs the side effect through a delegate bound once in the constructor. Game-free; the delegate is a
/// method of the Infrastructure adapter (it writes the config entry).
/// </summary>
internal sealed class MuteExecutor
{
    internal const string NotOwner = "not the owner", Applied = "applied";
    private readonly IOwnership _owner;
    private readonly Action<SoundSlider, float> _apply;

    internal MuteExecutor(IOwnership owner, Action<SoundSlider, float> apply)
    {
        if (RuleLog.IsOn()) RuleLog.Verbose($"MuteExecutor.MuteExecutor(owner={owner}, apply={apply})");
        if (owner == null) throw new ArgumentNullException("owner");
        if (apply == null) throw new ArgumentNullException("apply");
        _owner = owner;
        _apply = apply;
    }

    /// <summary>Expected outcome: Success with the applied level; Rejected (nothing applied) when this client is not the owner.</summary>
    internal OperationResponse<float> Apply(SoundSlider slider, float level)
    {
        if (RuleLog.IsOn()) RuleLog.Every("MuteExecutor.Apply", 2d, $"MuteExecutor.Apply(slider={slider}, level={level})");
        if (!_owner.IsOwner) { var returnedAt31 = OperationResponse<float>.Rejected(NotOwner); if (RuleLog.IsOn()) RuleLog.Every("MuteExecutor.Apply:31", 2d, $"MuteExecutor.Apply: returns {returnedAt31} (line 31)"); return returnedAt31; }
        _apply(slider, level);
        { var returnedAt33 = OperationResponse<float>.Success(level, Applied); if (RuleLog.IsOn()) RuleLog.Every("MuteExecutor.Apply:33", 2d, $"MuteExecutor.Apply: returns {returnedAt33} (line 33)"); return returnedAt33; }
    }
}
