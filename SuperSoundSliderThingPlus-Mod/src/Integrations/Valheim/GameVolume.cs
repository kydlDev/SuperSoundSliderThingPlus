using KydlCore.Common;
using UnityEngine;
using SuperSoundSliderThingPlus.Core;

namespace SuperSoundSliderThingPlus.Integrations.Valheim;

/// <summary>
/// The player's in-game volumes (Settings > Audio, saved as the floats "MasterVolume", "SfxVolume", "MusicVolume" in the game's preferences;
/// verified in the IL 2026-11-10). The game applies them as master x sfx to the mixer (AudioMan.SetSFXVolume) and master x music to MusicMan,
/// which is repeated here for a live change. Reads and writes the game's own storage; nothing else.
/// </summary>
internal static class GameVolume
{
    internal const string MasterKey = "MasterVolume", SfxKey = "SfxVolume", MusicKey = "MusicVolume";

    /// <summary>Expected outcome: the master volume 0..1 (1 when the player never changed it).</summary>
    internal static float Master() => Get(MasterKey);

    /// <summary>Expected outcome: the volume 0..1 stored under a preference key (1 when the player never changed it).</summary>
    internal static float Get(string key) => Mathf.Clamp01(PlatformPrefs.GetFloat(key, 1f));

    /// <summary>Expected outcome: stores one of the three volumes (master, sound effects, music) and applies all of them to the running game at once; the preferences file is saved by <see cref="Save"/>.</summary>
    internal static bool Write(string key, float volume)
    {
        float v = Mathf.Clamp01(volume);
        PlatformPrefs.SetFloat(key, v);
        Apply(true);
        ModLog.Verbose("GameVolume: " + key + " set to " + v.ToString("F3") + " (stored and applied)");
        return true;
    }

    /// <summary>
    /// Applies the stored volumes to the running game. The music is master x music. The SFX mixer gets master only while the mod runs
    /// (<paramref name="modOwnsSfx"/>): the sound effects slider is applied per sound by the mod's patches, so the game's own SFX volume
    /// must not attenuate a second time; without the mod (false) the game's own master x sfx applies.
    /// </summary>
    internal static void Apply(bool modOwnsSfx)
    {
        if (RuleLog.IsOn()) ModLog.Every("GameVolume.Apply", 2d, $"GameVolume.Apply(modOwnsSfx={modOwnsSfx})");
        float master = Master();
        AudioMan.SetSFXVolume(modOwnsSfx ? master : master * Get(SfxKey));
        MusicMan.m_masterMusicVolume = master * Get(MusicKey);
    }

    /// <summary>Applies only when the mixer drifted away (the game re-applies its own sfx volume when its slider moves). Expected outcome: true when it had to be set again.</summary>
    internal static bool Reassert()
    {
        if (RuleLog.IsOn()) ModLog.Every("GameVolume.Reassert", 2d, $"GameVolume.Reassert: game sfx volume={AudioMan.GetSFXVolume():0.###}, master={Master():0.###}");
        if (Mathf.Abs(AudioMan.GetSFXVolume() - Master()) < 0.001f) { if (RuleLog.IsOn()) ModLog.Every("GameVolume.Reassert:49", 2d, $"GameVolume.Reassert: returns false (line 49) - sfx volume {AudioMan.GetSFXVolume():0.###} already equals master {Master():0.###}"); return false; }
        Apply(true);
        { if (RuleLog.IsOn()) ModLog.Every("GameVolume.Reassert:51", 2d, $"GameVolume.Reassert: returns true (line 51) - set again to master {Master():0.###}"); return true; }
    }

    /// <summary>Writes the preferences to disk.</summary>
    internal static bool Save()
    {
        if (RuleLog.IsOn()) ModLog.Every("GameVolume.Save", 2d, $"GameVolume.Save: writing the preferences, sfx volume={AudioMan.GetSFXVolume():0.###}");
        PlatformPrefs.Save();
        { if (RuleLog.IsOn()) ModLog.Every("GameVolume.Save:59", 2d, $"GameVolume.Save: returns true (line 59) - preferences written, sfx volume {AudioMan.GetSFXVolume():0.###}"); return true; }
    }

    /// <summary>The volume the mixer plays SFX at right now (tests).</summary>
    internal static float Sfx() => AudioMan.GetSFXVolume();
}
