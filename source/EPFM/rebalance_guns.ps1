param(
    [string]$SourcePath = "D:\SteamLibrary\steamapps\common\Ultimate Admiral Dreadnoughts\Mods\Default_Files\UAD_Files\guns.csv",
    [string]$OutputPath = "D:\ultimateadmiraldreadmod\mod\csv\guns_rebalanced.csv"
)

# Historical gun data - accurate per caliber based on NavWeaps + Campbell + Friedman
# Each row: caliber, aimProgress, hitChanceCurver, hitChanceMult,
#           shellV_5 (muzzle velocity in m/s), range_5 (km max),
#           shellW_5 (kg basic shell)
$histData = @{
    1  = @{ cal=1;   aimProg=12;   aimLock=80;    hcc=0.40; hcm=0.80; mv=760;  range=4.0;  shell=0.45;  firerate=15;  baseW=80   }
    2  = @{ cal=2;   aimProg=10;   aimLock=70;    hcc=0.42; hcm=0.85; mv=700;  range=7.0;  shell=1.0;   firerate=12;  baseW=120  }
    3  = @{ cal=3;   aimProg=8;    aimLock=55;    hcc=0.45; hcm=0.95; mv=700;  range=8.0;  shell=5.7;   firerate=8;   baseW=160  }
    4  = @{ cal=4;   aimProg=6;    aimLock=40;    hcc=0.48; hcm=1.10; mv=853;  range=11.5; shell=11.3;  firerate=6;   baseW=200  }
    5  = @{ cal=4.7; aimProg=5;    aimLock=30;    hcc=0.50; hcm=1.15; mv=810;  range=12.0; shell=17.0;  firerate=5;   baseW=250  }
    6  = @{ cal=6;   aimProg=4.5;  aimLock=25;    hcc=0.52; hcm=1.30; mv=853;  range=15.0; shell=45.4;  firerate=4;   baseW=350  }
    7  = @{ cal=7.5; aimProg=4;    aimLock=22;    hcc=0.55; hcm=1.45; mv=853;  range=18.0; shell=70.0;  firerate=3;   baseW=500  }
    8  = @{ cal=8;   aimProg=3.5;  aimLock=20;    hcc=0.58; hcm=1.55; mv=914;  range=20.0; shell=117.0; firerate=2.5; baseW=700  }
    9  = @{ cal=9.2; aimProg=3;    aimLock=18;    hcc=0.60; hcm=1.65; mv=869;  range=24.0; shell=172.0; firerate=2;   baseW=900  }
    10 = @{ cal=10;  aimProg=3;    aimLock=18;    hcc=0.62; hcm=1.75; mv=853;  range=27.0; shell=250.0; firerate=2;   baseW=1200 }
    11 = @{ cal=11;  aimProg=3;    aimLock=15;    hcc=0.64; hcm=1.85; mv=853;  range=29.0; shell=340.0; firerate=2;   baseW=1500 }
    12 = @{ cal=12;  aimProg=2.5;  aimLock=14;    hcc=0.66; hcm=2.00; mv=853;  range=32.0; shell=385.0; firerate=1.8; baseW=1800 }
    13 = @{ cal=13.5;aimProg=2.5;  aimLock=12;    hcc=0.68; hcm=2.15; mv=753;  range=33.0; shell=574.0; firerate=1.5; baseW=2200 }
    14 = @{ cal=14;  aimProg=2.2;  aimLock=10;    hcc=0.70; hcm=2.30; mv=853;  range=36.0; shell=680.0; firerate=1.5; baseW=2500 }
    15 = @{ cal=15;  aimProg=2;    aimLock=10;    hcc=0.72; hcm=2.50; mv=800;  range=37.0; shell=879.0; firerate=1.5; baseW=3200 }
    16 = @{ cal=16;  aimProg=2;    aimLock=9;     hcc=0.74; hcm=2.70; mv=820;  range=38.7; shell=1225.0;firerate=1.5; baseW=4000 }
    17 = @{ cal=17;  aimProg=2;    aimLock=9;     hcc=0.75; hcm=2.85; mv=762;  range=36.0; shell=1020.0;firerate=1.2; baseW=4500 }
    18 = @{ cal=18;  aimProg=1.8;  aimLock=8;     hcc=0.78; hcm=3.10; mv=780;  range=42.0; shell=1460.0;firerate=1.2; baseW=5500 }
    19 = @{ cal=19;  aimProg=1.8;  aimLock=8;     hcc=0.80; hcm=3.30; mv=780;  range=42.0; shell=1485.0;firerate=1.0; baseW=6500 }
    20 = @{ cal=20;  aimProg=1.5;  aimLock=7;     hcc=0.82; hcm=3.50; mv=750;  range=44.0; shell=1700.0;firerate=0.8; baseW=7500 }
    21 = @{ cal=21;  aimProg=1.5;  aimLock=7;     hcc=0.85; hcm=3.70; mv=730;  range=45.0; shell=2000.0;firerate=0.6; baseW=9000 }
}

$lines = Get-Content $SourcePath
$output = @()
$calIdx = @{1=0;2=1;3=2;4=3;5=4;6=5;7=6;8=7;9=8;10=9;11=10;12=11;13=12;14=13;15=14;16=15;17=16;18=17;19=18;20=19;21=20}

foreach ($line in $lines) {
    if ($line.StartsWith('#') -or $line.StartsWith('@') -or $line.StartsWith('default') -or [string]::IsNullOrWhiteSpace($line)) {
        $output += $line
        continue
    }
    $fields = $line.Split(',')
    if ($fields.Count -lt 56 -or $fields[0] -notmatch '^\d+$') {
        $output += $line
        continue
    }
    $cal = [int]$fields[0]
    if (-not $histData.ContainsKey($cal)) {
        $output += $line
        continue
    }
    $d = $histData[$cal]

    # Column indices in UAD gun CSV:
    # 0:id, 1:caliber, 2:aimProgress, 3:lockedProgress, 13:hitChanceCurver, 14:hitChanceMult
    # 25-29:firerate_1..5, 30-34:shellW_1..5, 35-39:shellV_1..5, 40-44:range_1..5
    # 18:baseWeight, 20-24:barrelW_1..5, 46-50:accuracy_1..5, 51-55:penetration_1..5
    $fields[2] = $d.aimProg.ToString("F3")
    $fields[3] = $d.aimLock.ToString("F3")
    $fields[13] = $d.hcc.ToString("F4")
    $fields[14] = $d.hcm.ToString("F4")
    $fields[18] = $d.baseW.ToString("F1")
    $fields[25] = $d.firerate.ToString("F1")
    $fields[26] = ($d.firerate + 0.5).ToString("F1")
    $fields[27] = ($d.firerate + 1.0).ToString("F1")
    $fields[28] = ($d.firerate + 1.5).ToString("F1")
    $fields[29] = ($d.firerate + 2.0).ToString("F1")
    $fields[30] = ($d.shell * 0.85).ToString("F2")
    $fields[31] = ($d.shell * 0.90).ToString("F2")
    $fields[32] = ($d.shell * 0.95).ToString("F2")
    $fields[33] = ($d.shell * 1.00).ToString("F2")
    $fields[34] = ($d.shell * 1.05).ToString("F2")
    $v = $d.mv
    $fields[35] = ($v * 0.85).ToString("F1")
    $fields[36] = ($v * 0.90).ToString("F1")
    $fields[37] = ($v * 0.95).ToString("F1")
    $fields[38] = ($v * 1.00).ToString("F1")
    $fields[39] = ($v * 1.10).ToString("F1")
    $r = $d.range
    $fields[40] = ($r * 0.30).ToString("F1")
    $fields[41] = ($r * 0.50).ToString("F1")
    $fields[42] = ($r * 0.70).ToString("F1")
    $fields[43] = ($r * 0.85).ToString("F1")
    $fields[44] = ($r * 1.00).ToString("F1")
    for ($i = 0; $i -lt 5; $i++) {
        $acc = 0.90 + ($i * 0.025)
        $fields[46 + $i] = $acc.ToString("F4")
    }
    for ($i = 0; $i -lt 5; $i++) {
        $pen = 0.85 + ($i * 0.04)
        $fields[51 + $i] = $pen.ToString("F4")
    }
    $fields[20] = ($d.baseW * 0.30).ToString("F2")
    $fields[21] = ($d.baseW * 0.45).ToString("F2")
    $fields[22] = ($d.baseW * 0.60).ToString("F2")
    $fields[23] = ($d.baseW * 0.75).ToString("F2")
    $fields[24] = ($d.baseW * 0.90).ToString("F2")
    $output += ($fields -join ',')
}

$output | Out-File -FilePath $OutputPath -Encoding UTF8
Write-Host "Wrote $($output.Count) rebalanced gun lines"