param(
    [string]$SourcePath = "D:\SteamLibrary\steamapps\common\Ultimate Admiral Dreadnoughts\Mods\Default_Files\UAD_Files\guns.csv",
    [string]$BaselinePath = "D:\ultimateadmiraldreadmod\tools\gun_baselines.json",
    [string]$OutputPath = "D:\ultimateadmiraldreadmod\mod\csv\guns_rebalanced.csv"
)

# Rebalance guns.csv shell velocity + shell weight against researched baselines.
#
# IMPORTANT (fixes prior corruption): shellV/shellW (columns 30-39) are plain
# unquoted numbers, but the range columns (40+) use QUOTED thousand-separators
# like "22,400". We only modify columns 30-39 and reconstruct the tail verbatim,
# so quoted fields are never mangled.

$baselines = (Get-Content $BaselinePath -Raw | ConvertFrom-Json).guns
$lines = [System.IO.File]::ReadAllLines($SourcePath)

# Column layout (0-indexed) confirmed from the header + a data row dump:
#  0=id  1=caliberInch ...
#  25-29 = firerate_1..5
#  30-34 = shellW_1..5   (kg)
#  35-39 = shellV_1..5   (m/s)   -> grade 5 = muzzle velocity
#  40+ = range_* (QUOTED) + accuracy + penetration  -> leave untouched
$out = New-Object System.Collections.Generic.List[string]
$changed = 0

foreach ($line in $lines) {
    if ($line -match '^\s*#' -or $line -match '^\s*@' -or $line -match '^\s*default' -or $line.Trim() -eq '') {
        $out.Add($line)
        continue
    }
    # Split only the FIRST 41 fields (cols 0-40). Cols 0-39 are unquoted clean numeric.
    # We must capture the remainder (col 40 = "22,400" quoted, etc.) verbatim.
    $idx = -1
    $fieldCount = 0
    $i = 0
    $fields = New-Object System.Collections.Generic.List[string]
    # find first 41 commas to split off 41 fields (0..40), but col 40 starts a quote
    # so we actually only need 40 clean fields (0..39) before the quote.
    $positions = @()
    for ($j = 0; $j -lt $line.Length; $j++) {
        if ($line[$j] -eq ',') { $positions += $j; if ($positions.Count -ge 40) { break } }
    }
    if ($positions.Count -lt 40) {
        # not enough columns; keep line as-is
        $out.Add($line)
        continue
    }
    $split40 = $positions[39]   # position of the 40th comma (after shellV_5 = col 39)
    $head = $line.Substring(0, $split40)
    $tail = $line.Substring($split40 + 1)   # everything after the 40th comma (cols 40+)
    $headFields = $head -split ','

    if ($headFields.Count -lt 40 -or $headFields[0] -notmatch '^\d+$') {
        $out.Add($line)
        continue
    }
    $cal = [int]$headFields[0]
    $b = $baselines."$cal"
    if ($null -eq $b) { $out.Add($line); continue }

    # shellV: grade 1..5 scale 0.85,0.90,0.95,1.00,1.10 of MV (m/s)
    $v = $b.mv
    $vFactors = @(0.85, 0.90, 0.95, 1.00, 1.10)
    for ($k = 0; $k -lt 5; $k++) {
        $headFields[35 + $k] = [math]::Round($v * $vFactors[$k], 1)
    }
    # shellW: grade 1..5 scale 0.85,0.90,0.95,1.00,1.05 of shell mass (kg)
    $m = $b.shell
    $mFactors = @(0.85, 0.90, 0.95, 1.00, 1.05)
    for ($k = 0; $k -lt 5; $k++) {
        $headFields[30 + $k] = [math]::Round($m * $mFactors[$k], 2)
    }

    $out.Add((($headFields -join ',') + ',' + $tail))
    $changed++
}

[System.IO.File]::WriteAllLines($OutputPath, $out)
Write-Host "Wrote $($out.Count) lines; rebalanced $changed gun rows (shellV + shellW only)."