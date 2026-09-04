# Research Notes — Primary & Secondary Naval Reference Data

This document records the historical data used to calibrate the rework, and the
sources it was drawn from. It is intended to be a living provenance/calibration
reference. Game values are mapped into `params.csv` (accuracy/aiming) and
`penetration.csv` (armor penetration).

> **Note on Jane's Fighting Ships.** While Jane's (1919, 1946) is the canonical
> ship-reference series, its content is not freely licensable online, so ship
> characteristics here are instead sourced from the freely-available NavWeaps
> and Nathan Okun archives, which cite the same primary material (Burt, Campbell,
> Friedman, Raven & Roberts, etc.). Jane's is best consulted in print for
> authoritative ship-by-ship armor/gun tables.

---

## 1. Gunnery hit-rates (real combat)

The single most important calibration target: **what fraction of shells actually
hit a moving target in battle?**

### Jutland (1916) — the WW1 benchmark

| Fleet | ~Shells fired | ~Hits | Hit rate |
|---|---|---|---|
| British Battle Cruiser Fleet | ~6,320 | ~123 | **~1.4–1.5%** |
| German High Seas Fleet (overall) | — | — | **~3%** (some analyses 3.0–3.7%) |

- Most hits were scored at **8,000–16,000 yards**; opening long-range fire
  (15,000–19,000 yd) was largely ineffective (slow, poorly-ranged startup).
- Source: post-war British Naval Staff analyses; classic works by
  John Campbell (*Jutland: An Analysis of the Fighting*) and Arthur Marder.
  *(These are well-established published benchmarks; verify against the print
  sources before treating as absolute.)*

### USN WWII main-battery theoretical accuracy (experimental)

From the famous **USN Armor Piercing (AP) ballistic tables** (AMP Report 79.2R,
July 1944), hits vs an Iowa-sized target, best spotting:

| Range | 16"/50 Mk 7, % hits (broadside) | % (end-on) |
|---|---|---|
| 10,000 yd | 32.7% | — |
| 20,000 yd | 10.5% | 4.1% |
| 30,000 yd | 2.7% | 1.4% |

These are **theoretical/ideal** values and are explicitly "optimistic" vs combat.
Post-war BuOrd/CINCPAC analyses put realistic battleship main-battery combat
accuracy at **~1–3% beyond 20,000 yd**. The exceptional night action of
USS *Washington* vs *Kirishima* (9 hits / ~42 rounds in 3 min at ~18,500 yd under
radar direction) is cited as the anomaly, not the norm.

### Implication for the mod
- Close range (≤10,000 yd): 10–30% hit rates are realistic.
- Long range (≥20,000 yd): **1–3%**, not the vanilla game's far higher values.
- This justifies capping the game's long-range accuracy and steepening the
  range-accuracy falloff so late-campaign long-range fire is historically poor.

---

## 2. Big-gun vertical armor penetration (real values)

Penetration of US **Class "A" face-hardened** armor (or the source navy's
equivalent) at stated range. These are the calibration anchors for `penetration.csv`.

| Gun | 0 yd (muzzle) | 5,000 yd | 20,000 yd | 30,000 yd |
|---|---|---|---|---|
| US 16"/50 Mk 7 (Iowa), M8 2500 fps | — | 29.39 in (747 mm) | 20.04 in (509 mm) | 14.97 in (380 mm) |
| US 16"/45 Mk 6, M8 2300 fps | — | 26.60 in (676 mm) | 17.62 in (448 mm) | 12.77 in (324 mm) |
| German 38 cm SK C/34 (Bismarck) 2690 fps | 29.23 in (742 mm) | 10,000 m = 20.08 in (510 mm) | 18,000 m = 16.50 in (419 mm) | 27,000 m = 11.98 in (304 mm) |
| Japanese 46 cm/45 Type 94 (Yamato) 2559 fps | 34.01 in (864 mm) | — | 20,000 m = 19.43 in (494 mm) side / 4.30 in (109 mm) deck | 30,000 m = 14.19 in (360 mm) side / 7.43 in (189 mm) deck |

Source: NavWeaps.com gun pages (NavWeaps is Tony DiGiulian; these tables in turn
transcribe USN/BuOrd and captured German/Japanese ballistic data).

### Implication for the mod
The game's `penetration.csv` is already computed from Okun/de Marre expressions
(the DIP baseline), so it tracks these curves closely. The above anchors should
be used to **verify** the 16", 38 cm, and 46 cm rows at their respective caliber
slots (the game interpolates by caliber inch across 1"–21").

---

## 3. Armor quality / metallurgy (Nathan Okun)

Relative armor resistance is expressed through the **de Marre "C" coefficient**
(from German *G.Kdos. 100*, 1940, as tabulated by Okun). Higher "C" = more
resistant. Values are projectile/obliquity/thickness dependent, so only the
simplest cross-comparables are given:

- **German KC n/A (face-hardened)**: C ≈ **676–685** intact vs 38 cm APC; overall
  range **650–804** across projectile types.
- **German Wotan Hart (Wh, homogeneous)**: C ≈ **655–668**.
- **US STS / Class B (homogeneous)**: Okun notes de Marre "C" for Cr-Ni steels
  ≈ **1.2–1.25×** the 1890 French nickel-steel baseline.

### Implication for the mod
Okun deliberately publishes **no** single "% resistance vs Class A" number — it
depends on projectile, obliquity, and thickness. So the mod models armor-quality
differences as **relative modifiers** (the `taf_epfm_v4_*_multiplier` params in
`params_override.csv`) rather than attempting a fake single-equivalency, which is
the historically correct approach. Current relative modifiers (RHA=1.00,
CHA=0.85, FHA=0.95, STS=1.10, KC=1.05) are directionally consistent with Okun's
"C" ordering.

---

## 4. Fire-control accuracy (World War II systems)

From NavWeaps "History and Technology — The British HACS" (which also documents
the USN Mk 37 GFCS):

- **HACS (British, early-war)**: pre-war estimate ~136–178 4" shells per aircraft
  downed; **wartime reality 2,000–10,000 rounds/plane**.
- **USN Mk 37 GFCS + 5"/38**: ~1,000 rounds/plane (MT fuzes); substantially fewer
  with VT (proximity) fuzes.
- HACS assumed steady, level targets (speed/height/course constant) and used
  manual fuze-setting — it was fundamentally inferior to the tachymetric Mk 37.

### Implication for the mod
Supports the `taf_epfm_v5_fire_control_realism` feature: the USN/RN late-war
radar-directed systems (Mk 37, Type 285/275) should be markedly more accurate than
interwar/axis optical systems, and IJN (no radar integration) should carry the
largest penalty — exactly what `taf_epfm_v5_ijn_fire_control_nerf` (0.75) does.

---

## Sources

- **NavWeaps** (Tony DiGiulian), https://www.navweaps.com/ — gun data, penetration
  tables, HACS/Mk 37 history & technology articles.
- **Nathan Okun Naval Gun/Armor Resource**, hosted on NavWeaps
  (https://www.navweaps.com/index_nathan/) — FACEHARD / M79APCLC penetration
  programs, *Major Historical Naval Armor Penetration Formulae*, metallurgical
  tables. (Okun's material © his estate; non-commercial use only.)
- **John Campbell**, *Naval Weapons of World War Two* (1985), and
  *Jutland: An Analysis of the Fighting* (1986).
- **Norman Friedman**, *U.S. Naval Weapons* (1983) and *Naval Weapons of WW2*.
- **Garzke & Dulin**, *Battleships: Axis and Neutral Battleships in WW2* (1985).
- **R.A. Burt**, *British Battleships 1919–1939* (2012).
- **Raven & Roberts**, *British Battleships / Cruisers of WW2*.
- USN **BuOrd** reports and **AMP Report No. 79.2R** (July 1944) — US AP ballistic
  accuracy tables.