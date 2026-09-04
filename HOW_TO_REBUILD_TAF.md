# How to rebuild the patched `TweaksAndFixes.dll`

The bundled `Mods/TweaksAndFixes.dll` is **TAF 3.21.1** rebuilt with two small fixes
that prevent a game-breaking crash in custom battles. This documents how to reproduce
that build from source.

## The bug (and the fixes)

When a custom-battle enemy has no matching shared design, TAF falls back to stock
predefined designs. Two problems caused an unhandled `NullReferenceException` that
killed the loading coroutine (the game appeared to "hang" while generating a ship):

1. **`VesselEntity.FromBaseStore` prefix** (`Harmony/Ship.cs`, class
   `Patch_VesselEntityFromStore`): TAF loaded its custom gun-grade data from a store
   that stock predefs don't fully populate. Fixed by wrapping the prefix body in a
   try/catch and adding a `turretCalibers == null` guard.

2. **`UpdateLoadingCustomBattleM`** (`Harmony/BattleManager.cs`): the predef loop called
   `Ship.FromStore(pd, ...)` with a **null** `pd` when
   `PredefinedDesignsData.GetRandomShip(nation, type, year)` returned nothing for that
   nation/type/year. The game's native `VesselEntity.FromBaseStore` then NRE'd. Fixed by
   adding a `pd == null` guard that falls through to random ship generation.

Both patched files are included in this repository under `source/TAF/`.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or newer; built with 10.0).
- MelonLoader installed into the game folder (provides the `MelonLoader/` and
  `MelonLoader/Il2CppAssemblies/` reference assemblies).
- The game itself (UAD 1.7.0.0).

## Steps

1. Clone the upstream TAF source at the exact tag:

   ```powershell
   git clone --depth 1 --branch Minor_Relase_3.21.1 https://github.com/DukeDagor/UADRealismDIP.git taf
   ```

2. Replace the two files in the fresh clone with the patched versions from this repo:

   ```powershell
   Copy-Item source\TAF\Ship.cs          taf\TweaksAndFixes\Harmony\Ship.cs          -Force
   Copy-Item source\TAF\BattleManager.cs taf\TweaksAndFixes\Harmony\BattleManager.cs -Force
   Copy-Item source\TAF\TweaksAndFixes.csproj taf\TweaksAndFixes\TweaksAndFixes.csproj -Force
   ```

   > `TweaksAndFixes.csproj` adds one missing `<Reference>` —
   > `UnityEngine.AudioModule` — that upstream omitted but `SceneManager.cs` requires.

3. Build with `UAD_PATH` pointing at your game install:

   ```powershell
   $env:UAD_PATH = "D:\SteamLibrary\steamapps\common\Ultimate Admiral Dreadnoughts\"
   dotnet build taf\TweaksAndFixes\TweaksAndFixes.csproj -c Release
   ```

   The project's `CopyAssets` target automatically copies the built
   `TweaksAndFixes.dll` into `$(UAD_PATH)Mods\TweaksAndFixes.dll`.

## Verifying the fix

After relaunching, a custom battle with an enemy that has no shared design should log:

```
No predef for <nation> <type> (<year>), generating.
```

instead of producing a `NullReferenceException` / "Unhandled exception in coroutine".

---

## Upstream attribution

TAF is by **DukeDagor** (fork of NathanKell's UADRealism):
https://github.com/DukeDagor/UADRealismDIP

The `Minor_Relase_3.21.1` tag (commit `1672355`) is the exact basis of the shipped
binary. TAF development was officially concluded; these patches are a minimal,
localized compatibility fix and do not claim to continue that project.