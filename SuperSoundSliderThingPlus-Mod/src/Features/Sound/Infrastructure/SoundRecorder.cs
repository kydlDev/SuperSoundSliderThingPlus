using System.Collections.Generic;
using System.Text;
using SuperSoundSliderThingPlus.Features.Sound.Domain;
using SuperSoundSliderThingPlus.Core;
using KydlCore.Common;

namespace SuperSoundSliderThingPlus.Features.Sound.Infrastructure;

/// <summary>
/// Remembers every distinct (sound, owner) pair the patches have classified, with its category and how often it was seen. Read-only
/// evidence for the in-game tests and for the debug log: it is how the real names of the wolf pup / chicken love / shield generator
/// sounds are confirmed in game. Bounded, main thread only.
/// </summary>
internal static class SoundRecorder
{
    internal const int MaxEntries = 20000;   // was 1000: a full creature tour (160 creatures) sees about 1300 pairs, and a full recorder drops new ones silently
    private static readonly Dictionary<string, Entry> Seen = new Dictionary<string, Entry>();

    private sealed class Entry
    {
        internal SoundCategory Category;
        internal int Count;
    }

    /// <summary>How many different (sound, owner) pairs were recorded.</summary>
    internal static int Distinct => Seen.Count;

    /// <summary>Expected outcome: returns true the first time a pair is recorded (the caller logs it once), false for a repeat or when full.</summary>
    internal static bool Record(string sound, string owner, SoundCategory category)
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Record(sound={sound}, owner={owner}, category={category})");
        string key = sound + " | " + owner;
        Entry entry;
        if (Seen.TryGetValue(key, out entry))
        {
            entry.Count++;
            { var returnedAt37 = entry.Count == 1; if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Record: returns {returnedAt37} (line 37)"); return returnedAt37; }
        }
        bool added = Seen.Count < MaxEntries;
        if (added) Seen[key] = new Entry { Category = category, Count = 1 };
        { var returnedAt41 = added; if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Record: returns {returnedAt41} (line 41)"); return returnedAt41; }
    }

    /// <summary>How many different pairs were classified into the category.</summary>
    internal static int DistinctIn(SoundCategory category)
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"Entry.DistinctIn(category={category})");
        int n = 0;
        foreach (Entry e in Seen.Values)
        {
            if (e.Category == category) n++;
        }
        { var returnedAt53 = n; if (RuleLog.IsOn()) ModLog.Verbose($"Entry.DistinctIn: returns {returnedAt53} (line 53)"); return returnedAt53; }
    }

    /// <summary>Was a sound whose name contains the text recorded in the category?</summary>
    internal static bool Has(string soundPart, SoundCategory category)
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Has(soundPart={soundPart}, category={category})");
        foreach (KeyValuePair<string, Entry> pair in Seen)
        {
            if (pair.Value.Category == category && pair.Key.Contains(soundPart)) { if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Has: returns true (line 62) - inputs (soundPart={soundPart}, category={category})"); return true; }
        }
        { if (RuleLog.IsOn()) ModLog.Verbose($"Entry.Has: returns false (line 64) - inputs (soundPart={soundPart}, category={category})"); return false; }
    }

    /// <summary>One line per category, e.g. "WolfPup 2, ChickenLove 1" - for test details and logs.</summary>
    internal static string Summary()
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.Summary: {Seen.Count} recorded sound/owner pair(s)");
        var sb = new StringBuilder();
        foreach (SoundCategory c in System.Enum.GetValues(typeof(SoundCategory)))
        {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(c).Append(' ').Append(DistinctIn(c));
        }
        { var returnedAt77 = sb.ToString(); if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.Summary: returns {returnedAt77} (line 77)"); return returnedAt77; }
    }

    /// <summary>Every recorded pair as "sound | owner = Category x count", separated by "; " (test logs).</summary>
    internal static string List()
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.List: {Seen.Count} recorded sound/owner pair(s)");
        var sb = new StringBuilder();
        foreach (KeyValuePair<string, Entry> pair in Seen)
        {
            if (sb.Length > 0) sb.Append("; ");
            sb.Append(pair.Key).Append(" = ").Append(pair.Value.Category).Append(" x").Append(pair.Value.Count);
        }
        { var returnedAt90 = sb.ToString(); if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.List: returns {returnedAt90} (line 90)"); return returnedAt90; }
    }

    /// <summary>Forgets everything (tests).</summary>
    internal static bool Clear()
    {
        if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.Clear: forgetting {Seen.Count} recorded sound/owner pair(s)");
        Seen.Clear();
        { var returnedAt98 = Seen.Count == 0; if (RuleLog.IsOn()) ModLog.Verbose($"SoundRecorder.Clear: returns {returnedAt98} (line 98)"); return returnedAt98; }
    }
}
