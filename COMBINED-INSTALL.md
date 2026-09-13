# Combined install — Rework + Carrier air wings (playable output)

This branch is the **merged, playtested output** of the Rework (`main`) plus the
carrier mod. It mirrors the repo layout; install by copying each file to its
target (backs up: keep a copy of your current `Mods/`, `params.csv`,
`params_override.csv`, `epfm_data.csv` first). Requires UAD **1.7.0.0**,
MelonLoader (0.6.x–0.7.x), TAF 3.21.1 Default_Files baseline.

> Sources live elsewhere: EPFM/carrier-agnostic data on `main`, the carrier
> mod (code, hulls, wings) in
> `Ultimate-Admiral-Dreadnoughts-CV-Barebone-Modification`. This branch holds
> no source — only the merged install set, verified working 2026-09-13.

## File map

| Branch file | Install to | What it is |
|---|---|---|
| `Mods/CarrierMod.dll` | `<UAD>/Mods/` | Carrier air-wing runtime (ships planes, AA, replay guard) |
| `Mods/EPFM.dll` | `<UAD>/Mods/` | **v8 fixed build** (24 hooks; see main README patch notes) |
| `Mods/TweaksAndFixes.dll` | `<UAD>/Mods/` | TAF 3.21.1 + custom-battle crash null-guard |
| `Mods/*_override.csv` (7 files) | `<UAD>/Mods/` | Carrier hulls, `cv`/`plane` types, wing tubes, personalities, models, armor |
| `Mods/CarrierModConfig.csv` | `<UAD>/Mods/` | Carrier tuning knobs (auto-created; restart after edits) |
| `csv/params.csv` | `<UAD>/params.csv` | Rework params **plus `torpedo_ammo,99`** (wings need it; side effect: all ships get deep torpedo stowage) |
| `csv/params_override.csv` | `<UAD>/params_override.csv` | All EPFM switches ON + wing torpedo lines + `taf_epfm_trace,0` |
| `csv/epfm_data.csv` | `<UAD>/epfm_data.csv` | EPFM tables |
| `game_files/UAD_Files/guns.csv` | `<UAD>/Mods/Default_Files/UAD_Files/` | Historic ballistics (unchanged from main) |
| `game_files/UAD_Files/penetration.csv` | `<UAD>/Mods/Default_Files/UAD_Files/` | Okun/NavWeaps curves (unchanged from main) |
| `game_files/UAD_Files/aiPersonalities.csv` | `<UAD>/Mods/Default_Files/UAD_Files/` | Rework values **plus carrier `buildRatio(cv)` + aviation TechMods** |
| `game_files/UAD_Files/technologies.csv` | `<UAD>/Mods/Default_Files/UAD_Files/` | Plus carrier hull unlocks (`hull_strength_12/14/end`) |
| `game_files/UAD_Files/English.lng` | `<UAD>/Mods/Default_Files/UAD_Files/` | Plus carrier part names |

## Notes

- **Play-again guard**: after a carrier battle always answer **No** (the mod
  disables Yes for you). Replays freeze the battle loader on plane designs.
- **Known EPFM limitation**: `Ship.GetPenetration` has no runtime hook (would
  CTD — struct-param marshaling; see main README). The v4 scale switch is
  inert; data curves apply normally.
- **First campaign battle**: run one skirmish with a carrier first so the mod
  mints its shared plane design, then campaign carriers work.
- Logs: `MelonLoader/Logs/Mel*.log` (expect 24 `EPFM Patched` lines, zero
  `Failed to patch`) and `CarrierMod.log`.
