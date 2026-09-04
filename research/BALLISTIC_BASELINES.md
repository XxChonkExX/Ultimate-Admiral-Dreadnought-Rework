# Ballistic Baseline Tables — All Navies & Eras

Consolidated calibration data for the ship-designer gun/armor model. These are the
primary-source baselines tuners should use to set muzzle velocity, shell mass, max
range, penetration, and armor-quality for each caliber. Sourced from NavWeaps
(Tony DiGiulian) and the Nathan Okun / Robert Lundgren FACEHARD tables.

---

## 1. Armor eras (critical for 1890–1910 tuning)

Pre-dreadnought guns faced armor generations that were **weaker** than late-war KC
n/A. A gun's penetration figure is only meaningful against its era-appropriate armor.

| Armor | Era | Notes | Relative resistance |
|---|---|---|---|
| Compound | <1885 | wrought-iron face on steel back; weak backing | lowest |
| Harvey nickel-steel | 1890–91 | ~1" carburized face on homogeneous steel | higher than compound |
| Krupp Cemented (KC) | 1894– | deeper ~35% cement face | reliably shatters uncapped AP |
| KC n/A (improved) | 1930+ | ~10–15% tougher than era KC | highest |

**Tuning rule:** when a penetration table is stated "vs Harvey" or "vs iron," scale
down ~15–25% to compare against Krupp Cemented-era equivalent. Okun's era formulae
(de Marre nickel-steel exponent 0.714; Davis Harvey 0.625–0.667) reflect the weak,
shatter-prone *uncapped* AP of the 1890s — do not apply the later 0.2 weight-exponent
(FACEHARD) model to pre-1910 shells.

---

## 2. US Navy — FACEHARD penetration (US Class A, 1935–43)

Full tables in `RESEARCH_NOTES.md` section 3 and the source file
`navweaps.com/index_nathan/Penetration_United_States.php`. Summary anchors
(NL = Naval Limit / EFF, inches of Class A; Deck in inches):

| Gun (shell, fps) | 0 kyd | 10 kyd | 20 kyd | 30 kyd | 40 kyd |
|---|---|---|---|---|---|
| 16"/50 Mk 8 1-5 (2700 lb, 2500) | 33.6 / 0 | 25.5 / 1.7 | 19.1 / 3.5 | 14.8 / 5.3 | 11.9 / 14.0 |
| 16"/50 Mk 8 6-8 (2700 lb, 2500) | 37.3 / 0 | 28.3 / 1.7 | 21.2 / 3.5 | 16.5 / 5.3 | 12.8 / 14.0 |
| 16"/45 Mk 8 1-5 (2700 lb, 2300) | 30.3 / 0 | 22.7 / 1.8 | 16.9 / 3.6 | 13.0 / 6.5 | 10.0 / 13.8 |
| 14"/50 Mk 16 1-6 (1500 lb, 2700) | 30.8 / 0 | 20.9 / 1.3 | 14.1 / 2.6 | 10.1 / 4.5 | 8.3 / 8.1 |
| 14"/45 Mk 16 1-6 (1500 lb, 2600) | 29.4 / 0 | 19.5 / 1.3 | 12.9 / 2.6 | 8.9 / 4.8 | 7.4 / 7.4 |
| 12"/50 Mk 18 (1140 lb, 2500) | 25.2 / 0 | 17.7 / 1.2 | 12.2 / 2.5 | 9.2 / 4.3 | 6.5 / 10.3 |
| 8"/55 Mk 19 1-3 (260 lb, 2800) | 15.5 / 0 | 8.6 / 0.7 | 4.6 / 1.4 | 2.5 / 4.1 | — |
| 6"/47 Mk 35 1-8 (130 lb, 2500) | 9.5 / 0 | 4.4 / 0.65 | 2.4 / 1.8 | 1.3 / 3.7 | — |
| 5"/38 Mk 38 (55 lb, 2600) | 4.8 / 0 | 1.4 / 0.6 | — | — | — |

Full US gun list (23 guns) and per-mod rows are in `RESEARCH_NOTES.md`.

---

## 3. Germany (WW1–WW2)

| Gun | Cal | MV | Shell | Max range | Penetration (side/deck) |
|---|---|---|---|---|---|
| 38 cm SK C/34 (Bismarck) | 52 | 2,690 fps | 800 kg | 40,000 m | 510 mm @10 km; 364 mm @20 km; 308 mm @25 km; ~120 mm deck @30 km |
| 28 cm SK C/28 (Panzerschiffe) | 52 | 2,986 fps | 300 kg | 36,475 m | (no table; striker 611 m/s @10 km, 407 m/s @20 km) |
| 30.5 cm SK L/50 (WW1) | 50 | 2,805 fps | 405 kg | 32,000 m | 345 mm @10 km; 305 mm @12 km; 254 mm @14 km; 229 mm @16 km |
| 15 cm SK C/25 (5.9") | 60 | 3,150 fps | 45.5 kg | 25,700 m | 60 mm @3.2 km; 20 mm @11.2 km |
| 10.5 cm SK C/33 (4.1", AA/DP) | 65 | 2,952 fps | 15.1 kg | 17,700 m | — |
| 8.8 cm SK C/35 (3.46", U-boat) | 45 | 2,300 fps | 9.0 kg | 11,950 m | — |

**38 cm SK C/34 range table** (elev → range → striking vel → ToF):
4.9° → 10,000 m → 641 m/s → 13.9 s; 12.1° → 20,000 m → 511 m/s → 32.0 s;
22.4° → 30,000 m → 457 m/s → 55.5 s.

Sources: `navweaps.com/Weapons/WNGER_15-52_skc34.php`, `WNGER_11-52_skc28.php`,
`WNGER_12-50_skc12.php`, `WNGER_59-60_skc25.php`, `WNGER_41-65_skc33.php`,
`WNGER_88mm-45_skc35.php`.

---

## 4. Russia / USSR

| Gun | Cal | MV | Shell | Max range | Penetration (side/deck) |
|---|---|---|---|---|---|
| 305mm/52 (12") Pattern 1907 | 52 | 2,500 fps | 470.9 kg | 23,300 m | 352 mm / 17 mm @9.1 km; 207 / 60 @18.3 km; 127 / 140 @27.4 km |
| 180mm/57 (7.1") B-1-P | 57 | 3,018 fps | 97.5 kg | 37,800 m | (no table) |
| 130mm/50 (5.1") B-13 | 50 | 2,855 fps | 33.4 kg | 25,597 m | (no table) |
| 100mm/56 (3.9") B-34 | 56 | 2,953 fps | 15.6 kg | 22,241 m | (no table) |

Sources: `navweaps.com/Weapons/WNRussian_12-52_m1907.php`, `WNRussian_71-57_m1932.php`,
`WNRussian_51-50_m1936.php`, `WNRussian_39-56_m1940.php`.

**Note:** no dedicated Russian Okun FACEHARD page exists; Russian gun penetration is
sparse online. The 406mm/50 Pattern 1937 (Sovetskii Soyuz) is at
`WNRussian_16-50_m1937.php` for future expansion.

---

## 5. Pre-1910 / pre-dreadnought

### United Kingdom
| Gun | Cal | MV | Shell | Max range | Penetration (vs armor noted) |
|---|---|---|---|---|---|
| 13.5"/30 Mk I–IV | 30 | 2,016–2,099 fps | 1,250 lb | 12,620 yd | 28" iron @1,000 yd; 11" Krupp @3,000 yd |
| 12"/35 Mk VIII | 35 | 2,350 fps | 850 lb | 13,900 yd | 33" iron @1,000 yd; 12" steel @3,100 yd; 8.5" KC @10,000 yd |
| 10"/32 Mk I–IV | 32 | 2,040 fps | 500 lb | 10,100 yd | 20.4" iron @1,000 yd |
| 9.2"/50 Mk XI | 50 | 2,890 fps | 380 lb | 16,200 yd | 9.2" steel @5,200 yd; 7.0" KC @3,000 yd (30° obl) |
| 6"/45 BL Mk VII/VIII | 45 | 2,536 fps | 100 lb | 14,600 yd | 2.0" KC @3,000 yd (30° obl) |

### United States
| Gun | Cal | MV | Shell | Max range | Penetration |
|---|---|---|---|---|---|
| 13"/35 Mk 1/2 | 35 | 2,000 fps | 1,130 lb | 12,000 yd | 12.13"/10.09"/9.11" side @6/9/12 kyd (1905) |
| 12"/35 Mk 1/2 | 35 | 2,100 fps | 870 lb | 12,000 yd | (none) |
| 12"/40 Mk 3/4 | 40 | 2,800→2,400 fps | 870 lb | 19,000 yd | 14.6"/11.6"/9.4" Harvey @6/9/12k (old AP); 12.9"/10.7"/8.7" (new AP) |
| 8"/35 Mk 3/4 | 35 | 2,100 fps | 260 lb | 16,000 yd | 5.6"/4.1"/2.9" @6/9/12k yd |

### Germany
| Gun | Cal | MV | Shell | Max range | Penetration |
|---|---|---|---|---|---|
| 28 cm/40 SK L/40 | 40 | 2,690 fps | 530.7 lb | 20,590 yd | 6.3" side @13,120 yd |
| 24 cm/40 SK L/40 | 40 | 2,263 fps | 308.6 lb | 18,500 yd | (none) |

**Era note:** UK/US "Harvey" and "iron" figures overstate vs KC-equivalent by
~15–25% (see §1). Use these to calibrate the 1890–1910 designer's arc-era armor.

Source filenames: `WNBR_135-30_mk1.php`, `WNBR_12-35_mk8.php`, `WNBR_10-32_mk1-4.php`,
`WNBR_92-50_mk11.php`, `WNBR_6-45_mk7.php`, `WNUS_13-35_mk1.php`, `WNUS_12-35_mk1.php`,
`WNUS_12-40_mk3.php`, `WNUS_8-35_mk3.php`, `WNGER_11-40_skc04.php`, `WNGER_945-40_skc94.php`.

---

## 6. Calibration to the ship-designer knobs

The designer's gun model is parameterized in `guns.csv` (per-caliber: muzzle
velocity, shell weight, max range, drag, turret weight, barrel weight) and
`params.csv` (`gun_length_*`, `caliber_length_*`, `default_caliber_*` lerp min/max
for velocity/range/penetration/firerate). The tables above map as:

- **Muzzle velocity** → `shellV_1..5` (per grade) and `default_caliber_velocity_min/max`.
- **Shell mass** → `shellW_1..5` (per grade).
- **Max range** → `range_1..5` and `default_caliber_range_min/max`.
- **Penetration** → `penetration.csv` (mm side/deck per range, per caliber inch)
  and `gun_length_penetration_min/max` (barrel-length effect).
- **Armor era** → the armor-generation baseline (armor types per year) — the 1890s
  should use compound/Harvey, 1900s KC, 1930s KC n/A.

Barrel-length (caliber) effects are already tunable via `gun_length_velocity/range/
penetration/firerate/turret_weight` min/max pairs, so a 45-cal vs 50-cal version of
the same caliber can be made to spread velocity/penetration exactly as the tables
above show (e.g. 16"/45 = 2300 fps vs 16"/50 = 2500 fps; 14"/45 = 2600 vs 14"/50 =
2700 fps).

---

## Sources
- NavWeaps (Tony DiGiulian), https://www.navweaps.com/ — all gun pages + range tables.
- Nathan Okun & Robert Lundgren FACEHARD penetration tables,
  https://www.navweaps.com/index_nathan/Penetration_index.php (US/Japan/Britain/
  Germany/France/Italy per-navy pages).
- Nathan Okun, "Major Historical Naval Armor Penetration Formulae",
  https://www.navweaps.com/index_nathan/Hstfrmla.php — era armor coefficients.
- Companion research notes: `research/RESEARCH_NOTES.md` in this repository.
---

## 7. Barrel-length (caliber) calibration

Real same-caliber velocity spread vs barrel length (from the FACEHARD/range tables):

| Caliber | 45-cal MV | 50-cal MV | +Δ per +5 cal |
|---|---|---|---|
| 16" | 2300 fps (701 m/s) | 2500 fps (762 m/s) | **+8.7%** |
| 14" | 2600 fps (792 m/s) | 2700 fps (823 m/s) | **+3.8%** |
| 12" | 2400 fps (731) | 2500 fps (762) | ~+4% |

Game knobs (`params.csv`): `gun_length_velocity_min/max` = -0.25/+0.125 across the
±20-caliber `min/max_gun_length_mod` span. At +12.5% max over 20 cal, the model gives
~+3.1% per +5 cal — slightly conservative vs the real ~4–9%. If tuners want a closer
match, raise `gun_length_velocity_max` from 0.125 toward 0.18 (≈ +4.5% per +5 cal).

`gun_length_firerate_min/max` = 0.105/-0.18 already captures that longer barrels fire
slightly slower (correct sign). `gun_length_range` and `gun_length_penetration` are
already monotonic in the right direction; leave unless empirical tests show otherwise.

*Recommendation:* keep the ±20-caliber designer freedom, but for historical builds the
"default" caliber per class should sit near the real service lengths (e.g. battleship
main 45–50 cal, cruiser 50–60 cal, secondary 45–55 cal) — the `default_caliber_lerp_min/
max` band (5–70) already spans these.
