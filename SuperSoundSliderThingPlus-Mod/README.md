# SuperSoundSliderThingPlus

Client-side audio control for Valheim: a master volume, the music, and a mute slider for the sound groups that annoy you most.

## Features
- **Eight sliders** in the KY hub (F6), all *mute levels*: 0% = full loudness, 100% = muted.
- **Master**, **Music** and **Sound effects** are the game's own volumes seen from the other side. Move one here and the game's audio settings follow; move the game's slider and ours follows. The Sound effects slider changes more than the game's own one: it covers every sound effect that has no slider of its own, candles included.
- **World weather** (rain, wind, thunder), **Wolf pups**, **Shield generator** (also its humming wall), **Chicken love** and **Portals** each have a slider of their own.
- **Loudness matching** (on by default, switch it off in the config): every sound is measured once and scaled toward one target loudness, so loud sounds come down and quiet ones come up. Switch it off with `EqualizeLoudness = false` if you prefer the game's own balance.
- The first time the game starts with the mod, the five group sliders start at 50% so you hear them working; after that your values stand.
- Client-side only: nothing is sent to other players and nothing in the world changes.

## How to use
- Requires **KydlCore** (installed automatically by the mod manager).
- Press **F6**, open the **Sound** tab and drag the sliders. Changes apply at once and are saved.

## Configuration
`BepInEx/config/com.kydldev.SuperSoundSliderThingPlus.cfg` (or the in-game config manager):

| Section | Setting | Default | What it does |
|---------|---------|---------|--------------|
| General | Enable Mod | true | Switches the whole mod on / off while playing |
| General | Enable Game Menu Sounds | true | The three sliders of Game menu sounds work (off = they do nothing, the game's volumes stay as they are) |
| General | Enable Special Adjustments | true | The five sliders of Special adjustments work (off = those sounds are left alone) |
| General | EqualizeLoudness | true | Match the loudness of all sounds |
| Game menu sounds | Master mute | 0 | Master mute level (the game's master volume) |
| Game menu sounds | Music mute | 0 | Music mute level (the game's music volume) |
| Game menu sounds | Sound Effects mute | 0 | Sound effects without a slider of their own (the game's sound effects volume) |
| Special adjustments | Portal mute / Shield Generator mute / Weather mute / Wolf Pup mute / Chicken Love mute | 0 (50 after the first start) | Mute level of that group |
| DebugLog | Enable | false | Detailed lines in the BepInEx log for bug reports |

Edits to the file are picked up while the game runs.

## Multiplayer
Client-side: it works on any server. Every player who wants the sliders installs it for themselves.

## Known limits
- Menu sounds are reached by the Master slider only, and the music by Master and Music; the group sliders only change sound effects.

## Bug reports
Turn on `[DebugLog] Enable`, reproduce the problem, and attach `BepInEx/LogOutput.log`.

---

![KydlDev](https://raw.githubusercontent.com/kydlDev/.github/main/branding/KydlDev-banner-960x240.png)

## Get help and stay in touch
Questions, bug reports, ideas or mod requests? Come say hi in the **KydlDev Discord**: **https://discord.gg/tX2CcTNC4W**
Everyone is welcome, and it is the quickest way to reach me. Source code and issues: https://github.com/kydlDev
