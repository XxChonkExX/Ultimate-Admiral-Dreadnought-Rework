param(
    [string]$SourcePath = "D:\SteamLibrary\steamapps\common\Ultimate Admiral Dreadnoughts\Mods\Default_Files\UAD_Files\penetration.csv",
    [string]$BaselinePath = "D:\ultimateadmiraldreadmod\tools\pen_baselines.json",
    [string]$OutputPath = "D:\ultimateadmiraldreadmod\mod\csv\penetration.csv"
)

# Rebalance penetration.csv (belt/side + deck mm vs range) against Okun FACEHARD
# + NavWeaps anchors. Columns are plain unquoted numbers.

$baselines = (Get-Content $BaselinePath -Raw | ConvertFrom-Json).guns
$lines = [System.IO.File]::ReadAllLines($SourcePath)
$out = New-Object System.Collections.Generic.List[string]
$changed = 0

# Column layout (0-indexed):
# 0=id 1=caliberInch
# 2..12 = vert_0, vert_1, vert_5, vert_10, vert_15, vert_20, vert_25, vert_30, vert_35, vert_40, vert_50
# 13..19 = horiz_5, horiz_10, horiz_15, horiz_20, horiz_25, horiz_30, horiz_35, horiz_40, horiz_50
# (horiz columns = 13..19 = 7 columns: 5,10,15,20,25,30,35,40,50 -> wait, that's 9. Let me count from header.)
# Header: vert_0,vert_1,vert_5,vert_10,vert_15,vert_20,vert_25,vert_30,vert_35,vert_40,vert_50  (11 -> cols 2-12)
#        horiz_5,horiz_10,horiz_15,horiz_20,horiz_25,horiz_30,horiz_35,horiz_40,horiz_50 (9 -> cols 13-21)

$vertIdx = @(2,3,4,5,6,7,8,9,10,11,12)   # vert_0,1,5,10,15,20,25,30,35,40,50
$horizIdx = @(13,14,15,16,17,18,19,20,21) # horiz_5,10,15,20,25,30,35,40,50
# baseline arrays: vert has 11 entries (0,1,5,10,15,20,25,30,35,40,50)
# horiz has 11 entries too in my JSON; but csv only has 9 horiz columns (5,10,15,20,25,30,35,40,50)
# Map horiz JSON index -> csv: h5,h10,h15,h20,h25,h30,h35,h40,h50 = json horiz[0..8]? 
# Actually my JSON horiz array is 11 values for positions matching [0?]. To keep simple, my JSON
# horiz array maps to: [h5,h10,h15,h20,h25,h30,h35,h40,h50, ?, ?] -> I'll treat first 9 as the 9 columns.

foreach ($line in $lines) {
    if ($line -match '^\s*#' -or $line -match '^\s*@' -or $line -match '^\s*default' -or $line.Trim() -eq '') {
        $out.Add($line); continue
    }
    $f = $line -split ','
    if ($f.Count -lt 22 -or $f[0] -notmatch '^\d+$') { $out.Add($line); continue }
    $cal = [int]$f[0]
    $b = $baselines."$cal"
    if ($null -eq $b) { $out.Add($line); continue }

    for ($k = 0; $k -lt 11; $k++) {
        $f[$vertIdx[$k]] = [math]::Round($b.vert[$k], 1)
    }
    for ($k = 0; $k -lt 9; $k++) {
        $f[$horizIdx[$k]] = [math]::Round($b.horiz[$k], 1)
    }
    $out.Add(($f -join ','))
    $changed++
}

[System.IO.File]::WriteAllLines($OutputPath, $out)
Write-Host "Wrote $($out.Count) lines; rebalanced $changed penetration rows."