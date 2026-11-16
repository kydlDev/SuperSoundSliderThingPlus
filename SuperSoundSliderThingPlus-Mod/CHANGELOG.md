# Changelog

## 1.0.1

- Changed: the menus of this mod take their text sizes, text colours and font from KydlCore. Needs KydlCore 1.1.1 or newer.
- Changed: all the **Enable** switches are now together in `[General]` and have new names: `Enable mod` (was `EnableAll`) and `Enable Colours` (each was `Enable` in its own section). The values you had set are taken over; the old entries are removed from your config file.

### Testing
- Unit tests: 143 passed. Integration tests: 46 passed.
- Property-based tests (FsCheck, included in the unit tests above): 13 property checks with generated inputs.
- Coverage of the testable code: 100%. Mutation testing (Stryker): 100% of the mutants are caught by the tests.
- Performance benchmarks (BenchmarkDotNet): 5.
- Static analysis (SonarQube): no Medium or High issues.
- In game with DrakesTestLab: the full suite of 22 tests per run, run 3 times in a row on the release build, all green (the release requirement).

## 1.0.0
- First release: master, music, sound effects, world weather, wolf pups, shield generator, chicken love and portal sliders in the KY hub (F6).
- Master, Music and Sound effects follow the game's own audio settings both ways.
- Loudness matching (on by default).

