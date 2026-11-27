using System;
using KydlCore.Core;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Features.Sound.Execution;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Features.Sound.Application;

/// <summary>
/// Pipeline (command given, then the domain decision: clamp the level into 0..100) -> executor (writes the slider). Built once.
/// Game-free: tested with a fake ownership and a recording action (tests/Integration).
/// </summary>
internal sealed class SetMuteHandler : ICommandHandler<SetMuteCommand, float>
{
    internal const string NoCommand = "no command";
    private readonly OperationPipeline<SetMuteCommand, float> _pipeline;
    private readonly MuteExecutor _executor;

    internal SetMuteHandler(MuteExecutor executor, Action<string> trace)
    {
        if (RuleLog.IsOn()) RuleLog.Verbose($"SetMuteHandler.SetMuteHandler(executor={executor}, trace={trace})");
        if (executor == null) throw new ArgumentNullException("executor");
        _executor = executor;
        _pipeline = new OperationPipeline<SetMuteCommand, float>("SetMute", c => SoundMixRules.Level(c.Requested))
            .Validate("command given", c => ValidationResult.RejectUnless(c != null, NoCommand));
        _pipeline.Trace = trace;
    }

    /// <summary>Expected outcome: the executor's response when the rules accept the level, else the rules' Rejected response.</summary>
    public OperationResponse<float> Handle(SetMuteCommand command)
    {
        if (RuleLog.IsOn()) RuleLog.Verbose($"SetMuteHandler.Handle(command={command})");
        var decision = _pipeline.Run(command);
        { var returnedAt34 = decision.IsSuccess ? _executor.Apply(command.Slider, decision.Value) : decision; if (RuleLog.IsOn()) RuleLog.Verbose($"SetMuteHandler.Handle: returns {returnedAt34} (line 34)"); return returnedAt34; }
    }
}
