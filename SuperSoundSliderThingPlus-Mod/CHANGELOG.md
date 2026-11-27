# Changelog

## 1.0.2

- Changed: needs KydlCore 1.2.0 or newer; Jotunn is no longer needed (the version check on joining a server comes from KydlCore, as before: only when both sides have the mod the versions must match).
- Changed: much more detailed debug log (`[DebugLog] Enable`): every step says what it did and with which values, every early stop says why, and frequent lines are throttled so the log stays readable.
- Docs: the FAQ has 11 questions.

## 1.0.1

- Changed: the menus of this mod take their text sizes, text colours and font from KydlCore. Needs KydlCore 1.1.1 or newer.
- Changed: all the **Enable** switches are now together in `[General]` and have new names: `Enable Mod` (was `EnableAll`); the category switches are listed below. The values you had set are taken over; the old entries are removed from your config file.
- Changed: `Enable Mod` is now the first setting of `[General]`, in the F1 menu and in the F6 hub (your value is taken over).
- Changed: the sliders are in two new categories: **Game menu sounds** (`Master mute`, `Music mute`, `Sound Effects mute`) and **Special adjustments** (`Portal mute`, `Shield Generator mute`, `Weather mute`, `Wolf Pup mute`, `Chicken Love mute`). Each category has its own switch in `[General]` (`Enable Game Menu Sounds`, `Enable Special Adjustments`). Your values are taken over.
- Changed: the separate **Sound** tab in the F6 hub is gone. The sliders were shown twice; they are now only in the **Game menu sounds** and **Special adjustments** tabs (same values, nothing to do).

### Testing
- Unit tests: 159 passed. Integration tests: 47 passed.
- Property-based tests (FsCheck, included in the unit tests above): 13 property checks with generated inputs.
- Coverage of the testable code: 100%. Mutation testing (Stryker): 100% of the mutants are caught by the tests.
- Performance benchmarks (BenchmarkDotNet): 5.
- Static analysis (SonarQube): no Medium or High issues.
- In game with DrakesTestLab: the full suite of 22 tests per run, run 3 times in a row on the release build, all green (the release requirement).

## 1.0.0
- First release: master, music, sound effects, world weather, wolf pups, shield generator, chicken love and portal sliders in the KY hub (F6).
- Master, Music and Sound effects follow the game's own audio settings both ways.
- Loudness matching (on by default).

