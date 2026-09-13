# Ultimate Admiral: Dreadnoughts — Rework

A historical-balance rework for *Ultimate Admiral: Dreadnoughts* (UAD) v1.7.0.0 that
removes the AI's hard-coded advantages and rebalances combat around real-world naval
data, while fixing a game-breaking custom-battle crash. Opens much of the game to 
customization and editing, offering options to tailor the game to your liking. 
This is the UAD: Rework/EPFM (Even Playing Field Mod). Enjoy!

This project builds on, and would not exist without, the excellent work of others —
see **[Credits](#credits)**.

Edit: A sub-branch has been released that includes my barebones CV Carrier mod if you care to check it out.
---

## What this does

### 1. Removes AI advantages (equal playing field)
The vanilla game grants the AI hidden bonuses. This mod neutralizes them:

| System | Change |
|---|---|
| AI economy | Difficulty income/GDP multipliers set to `1.0` (was up to `1.3`) |
| AI tech/research | Difficulty tech multiplier set to `1.0` (was `1.1`); TAF tech-priority randomization disabled |
| AI aggression/tension | Difficulty multipliers set to `1.0` (was up to `1.75`) |
| AI autoresolve | AI power-estimation exponent equalized to the player value |
| AI replacement money | New AI opponents no longer receive free money |
| AI ship stats | `aiTechMod`, `aiTrainingMod`, `aiShipyardMod`, `aiTrMod`, `aiOffense`, `aiDefense` normalized to `1.0` across all 22 AI personalities |
| AI perfect aim | Runtime patches normalize AI accuracy/aiming/reload/damage-control skill values |
| AI fog-of-war bypass | AI ships now respect fog of war like the player |
| AI movement | Max speed / acceleration / crew quarters nerfs equalized |
| Major durability | `lose_provinces_threshold` 0.3 → 0.1: majors survive losing a capital/home territory (French-WWII model) and dissolve only when essentially annihilated, so the player conquers collapsed majors deliberately rather than by RNG |

### 2. Historical rebalance
Combat and ballistics tuned against real-world references (NavWeaps, Campbell,
Friedman, Garzke & Dulin, Burt, Nathan Okun / Robert Lundgren). Changes:
- `guns.csv` — historic **muzzle velocity** and **shell mass** per caliber (1"–21"),
  anchored to real service rounds (e.g. 16" = 1225 kg @ 762 m/s; 14" = 680 kg @
  823 m/s).
- `penetration.csv` — belt and deck **armor penetration curves** per caliber,
  calibrated to Okun/Lundgren FACEHARD tables (US Class A) and NavWeaps
  side-penetration data. The stock/DIP values were ~1.25–1.9× over-stated at close
  range; now they match the physical tables.
- Runtime patches cover (unchanged from prior work):
  - Armor quality modifiers (RHA / CHA / FHA / STS)
  - Shell construction effects (AP / APC / APCBC / APCR / APHE / SAP / HE)
  - Yaw-based effective armor and smooth ricochet curves (War Thunder style)
  - Historical turret loading / traverse, propulsion, and fire-control accuracy
  - Historical torpedo ballistics (incl. Japanese Type 93 "Long Lance" range)
  - IJN fire-control penalty vs. USN/RN (no radar integration)

### 3. Fixes a game-breaking crash
UAD custom battles stall indefinitely ("computer gets stuck generating a ship")
because of a null-reference in TAF when the enemy falls back to a stock predefined
design that doesn't exist for that nation/type/year. The bundled
`TweaksAndFixes.dll` is TAF 3.21.1 rebuilt with a null-guard so the enemy
auto-generates instead of crashing. See `source/TAF/` and
[`HOW_TO_REBUILD_TAF.md`](HOW_TO_REBUILD_TAF.md).

---

## Repository layout

```
.
├── Mods/
│   ├── EPFM.dll                  # Even Playing Field Mod (Harmony patches)
│   └── TweaksAndFixes.dll        # TAF 3.21.1 + custom-battle crash fix
├── csv/
│   ├── params.csv                # AI difficulty multipliers neutralized
│   ├── params_override.csv       # EPFM + TAF runtime configuration
│   ├── epfm_data.csv             # EPFM shell/armor/ricochet tuning tables
│   └── aiPersonalities.csv       # AI stat advantages normalized to 1.0
├── source/
│   ├── EPFM/                     # EPFM.cs, EPFM.csproj, rebalance_guns.ps1
│   └── TAF/                      # Patched TAF sources (Ship.cs, BattleManager.cs, csproj)
├── HOW_TO_REBUILD_TAF.md
└── README.md
```

---

## Installation

1. Install **MelonLoader 0.6.6** and **TweaksAndFixes (TAF) 3.21.1** into the game
   folder (see the [TAF wiki](https://github.com/DukeDagor/UADRealismDIP/wiki) for the
   full setup, including Il2CppInterop dependencies).
2. Copy `Mods/EPFM.dll` → `<UAD>/Mods/EPFM.dll`
3. Copy `Mods/TweaksAndFixes.dll` → `<UAD>/Mods/TweaksAndFixes.dll` (overwrite the stock TAF build)
4. Copy `csv/params.csv` → `<UAD>/params.csv`
5. Copy `csv/params_override.csv` → `<UAD>/params_override.csv`
6. Copy `csv/epfm_data.csv` → `<UAD>/epfm_data.csv`
7. Copy `csv/aiPersonalities.csv` → `<UAD>/Mods/Default_Files/UAD_Files/aiPersonalities.csv`
8. Copy `game_files/UAD_Files/guns.csv` → `<UAD>/Mods/Default_Files/UAD_Files/guns.csv`
   (historic muzzle velocity + shell mass per caliber)
9. Copy `game_files/UAD_Files/penetration.csv` → `<UAD>/Mods/Default_Files/UAD_Files/penetration.csv`
   (Okun/NavWeaps armor penetration curves)
10. Launch the game.

> **Note:** this mod was tested against UAD **1.7.0.0** (the final version of the game)
> and TAF **3.21.1**. `params.csv` and `aiPersonalities.csv` are treated as
> data-file overrides; deleting them restores the vanilla/DIP defaults.

---

## Configuration

Runtime behavior is controlled by `csv/params_override.csv`. Each EPFM feature has a
`taf_epfm_*` switch — set `1` to enable, `0` to disable. Examples:

```
taf_epfm_enabled,1                          # master enable
taf_epfm_equalize_ai_accuracy,1             # remove AI accuracy cheat
taf_epfm_equalize_fog_of_war,1              # AI respects fog of war
taf_epfm_equalize_ai_movement,1             # AI speed/accel nerf
taf_epfm_equalize_ai_economy,1              # AI economy rebalance
taf_epfm_v3_yawArmor,1                      # War-Thunder-style yaw armor
taf_epfm_v4_penetration_scale,1.0           # global penetration multiplier
taf_epfm_v5_turret_realism,1                # historical turret rates
taf_epfm_v5_fire_control_realism,1          # historical fire-control
taf_epfm_trace,0                            # diagnostic: log every patch entry (one run only)
```

> **Note:** EPFM reads these switches live from the installed
> `params_override.csv` (reloaded when the file changes) — the game has no
> `GameData.Instance` singleton, so the config never depended on it.

---

## Runtime patch notes (EPFM v8)

Audited against the 1.7.0.0 binary (`dump.cs`). All 25 hook targets verified by
(name, arity, staticness); every postfix is `public static`, so lookups use
`Public` flags. AI detection probes per type (`isAi` field → `isAi` property →
`isAiControlled` property) because Il2Cpp instance fields are invisible to
classic reflection at runtime.

**Hard rule: never patch an original with struct/`Nullable` parameters.**
`Ship.GetPenetration` (takes `Nullable<ShellType>` + `Nullable<Vector3>`) is
compiled out for exactly this reason — crash-dump proven: Harmony's Il2Cpp
wrapper Memmove-copies those structs per call, and turret-click stat cards
invoke it with null nullables (no target in preview), AV-ing the runtime
(`c0000005` in coreclr, `ExecutionEngineException`). Safe parameter classes:
primitives, `string`, `object`, references, generic collections. The v4
penetration multiplier therefore has no runtime effect until a safe
application point is found; the `penetration.csv` data curves are unaffected.

---

## Credits

This mod is a derivative work and depends on the following projects and references.

### Mod frameworks (code that this mod links against, builds on, and patches)
- **[Tweaks And Fixes (TAF)](https://github.com/DukeDagor/UADRealismDIP)** by
  **DukeDagor** (fork of NathanKell's UADRealism) — the foundational Unity/Il2Cpp
  modding framework and data-file override system. This project ships a
  rebuilt `TweaksAndFixes.dll` (tag `Minor_Relase_3.21.1`) with a small compatibility
  fix, and its `Default_Files` data are used as the canonical baseline.
  - Original foundation by **NathanKell**.
- **[Dreadnought Improvement Project (DIP)](https://github.com/brothermunro/Dreadnought-Improvement-Project)**
  by **Brother Munro** — the gameplay/balance overhaul whose `Default_Files`
  (branch `DIP-The-Great-Game`) provide the baseline data files this rework layers on.
- **[MelonLoader](https://github.com/LavaGang/MelonLoader)** and
  **[0Harmony](https://github.com/pardeike/Harmony)** — the .NET runtime patcher used
  by both TAF and EPFM.
- **[Il2CppInterop](https://github.com/BepInEx/Il2CppInterop)** — managed<->Il2Cpp
  interop layer.

### Reverse-engineering tooling
- **[Il2CppDumper](https://github.com/Perfare/Il2CppDumper)** by Perfare — used to
  recover class/method definitions (RVA map) from the IL2CPP `GameAssembly.dll`.
- **[Ghidra](https://github.com/NationalSecurityAgency/ghidra)** (NSA) — used to
  decompile `GameAssembly.dll` and locate the AI-advantage constants.
- **[ILSpy / ilspycmd](https://github.com/icsharpcode/ILSpy)** — used to decompile
  `TweaksAndFixes.dll` while diagnosing the custom-battle crash.

### Historical reference data
Ballistics, armor, penetration, fire-control, and torpedo values are derived from:
- **[NavWeaps](http://www.navweaps.com/)** — Tony DiGiulian's naval weapons database.
- **John Campbell**, *Naval Weapons of World War Two* (Naval Institute Press, 1985).
- **Norman Friedman**, *Naval Weapons of World War II* (1983).
- **Garzke & Dulin**, *Battleships: Axis and Neutral Battleships in World War II* (1985).
- **R.A. Burt**, *British Battleships 1919–1939* (2012).
- **War Thunder** — underyling inspiration for the yaw-armor and ricochet models.

### Thank you
This would not exist without the UAD modding community — especially the TAF and DIP
authors, whose open-source work made the entire endeavor possible. Any errors in this
rework are ours, not theirs.

---

## License

The EPFM patches and this repository are provided for non-commercial personal use.
`TweaksAndFixes.dll` is built from TAF, which is licensed per its own repository
(`DukeDagor/UADRealismDIP`); see that project for its license terms. Historical data
sources remain the property of their respective authors.

UAD is © Game Labs; this is an unofficial fan mod and is not affiliated with or
endorsed by Game Labs.
