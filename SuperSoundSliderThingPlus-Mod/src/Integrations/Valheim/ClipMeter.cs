using System.Collections.Generic;
using KydlCore.Common;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Integrations.Valheim;

/// <summary>
/// Learns how loud each AudioClip is while it plays. Most game clips are compressed in memory or streamed, so their samples cannot be read
/// (AudioClip.GetData fails - found in game 2026-11-11); AudioSource.GetOutputData is the source's output AFTER its volume (verified in game: half
/// the volume gives half the RMS), so each block is divided by the volume the source played at to get the clip's own level. One estimate per clip,
/// remembered by instance id, bounded. Reads audio only, never changes it. Main thread only.
/// </summary>
internal static class ClipMeter
{
    internal const float Unknown = -1f;
    internal const int MaxClips = 4000, BlockSize = 1024;
    private static readonly Dictionary<int, LoudnessEstimate> Estimates = new Dictionary<int, LoudnessEstimate>();
    private static readonly float[] Block = new float[BlockSize];

    /// <summary>How many clips have an estimate.</summary>
    internal static int Count => Estimates.Count;

    /// <summary>Expected outcome: adds one block of the source's output to its clip's estimate (true) or does nothing (false: no clip, not playing, volume too low to divide by, full).</summary>
    internal static bool Observe(AudioSource source, float playedVolume)
    {
        if (source == null || source.clip == null || !source.isPlaying || playedVolume < 0.01f) { if (RuleLog.IsOn()) ModLog.Every("ClipMeter.Observe:28", 2d, $"ClipMeter.Observe: returns false (line 28) - inputs (source={source}, playedVolume={playedVolume})"); return false; }
        LoudnessEstimate estimate = EstimateFor(source.clip, true);
        if (estimate == null || !estimate.WantsMore) { if (RuleLog.IsOn()) ModLog.Every("ClipMeter.Observe:30", 2d, $"ClipMeter.Observe: returns false (line 30) - inputs (source={source}, playedVolume={playedVolume})"); return false; }
        source.GetOutputData(Block, 0);
        var rms = LoudnessRules.Rms(Block, Block.Length);
        if (!rms.IsSuccess) { if (RuleLog.IsOn()) ModLog.Every("ClipMeter.Observe:33", 2d, $"ClipMeter.Observe: returns false (line 33) - inputs (source={source}, playedVolume={playedVolume})"); return false; }
        bool added = estimate.Add(rms.Value / playedVolume);
        if (added && estimate.Blocks == LoudnessEstimate.MaxBlocks)
            ModLog.Verbose("ClipMeter: '" + source.clip.name + "' learned, rms " + estimate.Rms.ToString("F4") + " from " + estimate.Blocks + " blocks");
        return added;
    }

    /// <summary>Expected outcome: the clip's level once enough was heard, otherwise <see cref="Unknown"/>.</summary>
    internal static float RmsOf(AudioClip clip)
    {
        if (RuleLog.IsOn()) ModLog.Every("ClipMeter.RmsOf", 2d, $"ClipMeter.RmsOf(clip={clip})");
        LoudnessEstimate estimate = clip == null ? null : EstimateFor(clip, false);
        { var returnedAt45 = estimate != null && estimate.IsReady ? estimate.Rms : Unknown; if (RuleLog.IsOn()) ModLog.Every("ClipMeter.RmsOf:45", 2d, $"ClipMeter.RmsOf: returns {returnedAt45} (line 45)"); return returnedAt45; }
    }

    /// <summary>Forgets every estimate (tests).</summary>
    internal static bool Clear()
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"ClipMeter.Clear: forgetting {Estimates.Count} clip estimate(s)");
        Estimates.Clear();
        { var returnedAt53 = Estimates.Count == 0; if (RuleLog.IsOn()) ModLog.Verbose($"ClipMeter.Clear: returns {returnedAt53} (line 53)"); return returnedAt53; }
    }

    private static LoudnessEstimate EstimateFor(AudioClip clip, bool create)
    {
        if (RuleLog.IsOn()) ModLog.Every("ClipMeter.EstimateFor", 2d, $"ClipMeter.EstimateFor(clip={clip}, create={create})");
        int id = clip.GetInstanceID();
        LoudnessEstimate estimate;
        if (Estimates.TryGetValue(id, out estimate)) { var returnedAt61 = estimate; if (RuleLog.IsOn()) ModLog.Every("ClipMeter.EstimateFor:61", 2d, $"ClipMeter.EstimateFor: returns {returnedAt61} (line 61)"); return returnedAt61; }
        if (!create || Estimates.Count >= MaxClips) { if (RuleLog.IsOn()) ModLog.Every("ClipMeter.EstimateFor:62", 2d, $"ClipMeter.EstimateFor: returns null (line 62) - inputs (clip={clip}, create={create})"); return null; }
        estimate = new LoudnessEstimate();
        Estimates[id] = estimate;
        { var returnedAt65 = estimate; if (RuleLog.IsOn()) ModLog.Every("ClipMeter.EstimateFor:65", 2d, $"ClipMeter.EstimateFor: returns {returnedAt65} (line 65)"); return returnedAt65; }
    }
}
