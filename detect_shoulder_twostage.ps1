# Two-stage shoulder detection algorithm
# Stage 1: Coarse detection with large window
# Stage 2: Fine detection with small window in the region

$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

try {
    $workbook = $excel.Workbooks.Open('C:\Users\Alex\Documents\repos\TPC\PNTZ.Mufta\doc\curve.xlsx')
    $worksheet = $workbook.Sheets.Item(3)

    $rows = $worksheet.UsedRange.Rows.Count

    $moments = @()
    $turns = @()
    $times = @()

    for($i=2; $i -le $rows; $i++) {
        $time = $worksheet.Cells.Item($i, 1).Value2
        $turn = $worksheet.Cells.Item($i, 2).Value2
        $moment = $worksheet.Cells.Item($i, 3).Value2

        if ($moment -ne $null -and $time -ne $null) {
            $times += [double]$time
            $turns += [double]$turn
            $moments += [double]$moment
        }
    }

    Write-Host "=== Two-Stage Shoulder Detection ==="
    Write-Host "Total points: $($moments.Count)"
    Write-Host ""

    # Find maximum and cut off unloading
    $maxMoment = ($moments | Measure-Object -Maximum).Maximum
    $maxIndex = [array]::IndexOf($moments, $maxMoment)
    $analyzeEnd = $maxIndex

    Write-Host "Max torque: $maxMoment Nm at index $maxIndex"
    Write-Host ""

    # ========== STAGE 1: Coarse detection with large window ==========
    Write-Host "=== STAGE 1: Coarse Detection (window=200) ==="

    $largeWindow = 200
    $avgDerivativesLarge = @()
    $windowCenters = @()

    for($i=$largeWindow; $i -lt ($analyzeEnd - $largeWindow); $i += 5) {
        $sum = 0
        $count = 0
        for($j=($i - $largeWindow/2); $j -lt ($i + $largeWindow/2 - 1); $j++) {
            $dt = $times[$j+1] - $times[$j]
            if ($dt -gt 0) {
                $sum += ($moments[$j+1] - $moments[$j]) / $dt
                $count++
            }
        }

        if ($count -gt 0) {
            $avgDerivativesLarge += $sum / $count
            $windowCenters += $i
        }
    }

    # Identify free threading phase
    $startIdx = [int]($avgDerivativesLarge.Count * 0.2)
    $endIdx = [int]($avgDerivativesLarge.Count * 0.7)
    $freeThreadingDerivatives = $avgDerivativesLarge[$startIdx..$endIdx]
    $baselineAvg = ($freeThreadingDerivatives | Measure-Object -Average).Average

    $variance = 0
    foreach($d in $freeThreadingDerivatives) {
        $variance += [Math]::Pow($d - $baselineAvg, 2)
    }
    $baselineStd = [Math]::Sqrt($variance / $freeThreadingDerivatives.Count)

    Write-Host "Baseline: $([Math]::Round($baselineAvg, 2)) +- $([Math]::Round($baselineStd, 2)) Nm/sec"

    # Find approximate shoulder region
    $threshold = $baselineAvg + 3 * $baselineStd
    $coarseShoulderIdx = -1

    for($i=$endIdx; $i -lt $avgDerivativesLarge.Count; $i++) {
        if ($avgDerivativesLarge[$i] -gt $threshold) {
            $coarseShoulderIdx = $i
            break
        }
    }

    if ($coarseShoulderIdx -eq -1) {
        $threshold = $baselineAvg + 2 * $baselineStd
        for($i=$endIdx; $i -lt $avgDerivativesLarge.Count; $i++) {
            if ($avgDerivativesLarge[$i] -gt $threshold) {
                $coarseShoulderIdx = $i
                break
            }
        }
    }

    if ($coarseShoulderIdx -eq -1) {
        Write-Host "ERROR: Coarse shoulder not found!"
        exit
    }

    $coarseShoulderDataIdx = $windowCenters[$coarseShoulderIdx]

    Write-Host "Coarse shoulder region found:"
    Write-Host "  Center index: $coarseShoulderDataIdx"
    Write-Host "  Time: $([Math]::Round($times[$coarseShoulderDataIdx], 2)) sec"
    Write-Host "  Torque: $([Math]::Round($moments[$coarseShoulderDataIdx], 0)) Nm"
    Write-Host "  Average derivative: $([Math]::Round($avgDerivativesLarge[$coarseShoulderIdx], 1)) Nm/sec"
    Write-Host ""

    # ========== STAGE 2: Fine detection with small window ==========
    Write-Host "=== STAGE 2: Fine Detection (window=10) ==="

    # Analyze region around coarse detection
    $regionStart = [Math]::Max(0, $coarseShoulderDataIdx - $largeWindow)
    $regionEnd = [Math]::Min($analyzeEnd - 1, $coarseShoulderDataIdx + $largeWindow/2)

    Write-Host "Analyzing region: indices $regionStart to $regionEnd"
    Write-Host ""

    $smallWindow = 10
    $avgDerivativesSmall = @()
    $smallWindowCenters = @()

    for($i=($regionStart + $smallWindow); $i -lt ($regionEnd - $smallWindow); $i++) {
        $sum = 0
        $count = 0
        for($j=($i - $smallWindow/2); $j -lt ($i + $smallWindow/2 - 1); $j++) {
            $dt = $times[$j+1] - $times[$j]
            if ($dt -gt 0) {
                $sum += ($moments[$j+1] - $moments[$j]) / $dt
                $count++
            }
        }

        if ($count -gt 0) {
            $avgDerivativesSmall += $sum / $count
            $smallWindowCenters += $i
        }
    }

    # Find where small-window derivative starts exceeding threshold
    $fineShoulderIdx = -1

    for($i=0; $i -lt $avgDerivativesSmall.Count; $i++) {
        if ($avgDerivativesSmall[$i] -gt $threshold) {
            $fineShoulderIdx = $i
            break
        }
    }

    if ($fineShoulderIdx -eq -1) {
        Write-Host "Fine shoulder not found, using coarse result"
        $fineShoulderDataIdx = $coarseShoulderDataIdx
    } else {
        $fineShoulderDataIdx = $smallWindowCenters[$fineShoulderIdx]
    }

    Write-Host "=== SHOULDER POINT DETECTED ==="
    Write-Host "  Index: $fineShoulderDataIdx of $($moments.Count)"
    Write-Host "  Time: $([Math]::Round($times[$fineShoulderDataIdx], 3)) sec"
    Write-Host "  Turns: $([Math]::Round($turns[$fineShoulderDataIdx], 5)) turns"
    Write-Host "  Torque: $([Math]::Round($moments[$fineShoulderDataIdx], 1)) Nm"

    if ($fineShoulderIdx -ge 0) {
        Write-Host "  Fine derivative: $([Math]::Round($avgDerivativesSmall[$fineShoulderIdx], 1)) Nm/sec"
    }

    Write-Host ""
    Write-Host "Comparison:"
    Write-Host "  Coarse detection: index $coarseShoulderDataIdx, time $([Math]::Round($times[$coarseShoulderDataIdx], 2)) sec"
    Write-Host "  Fine detection:   index $fineShoulderDataIdx, time $([Math]::Round($times[$fineShoulderDataIdx], 2)) sec"
    Write-Host "  Difference: $($fineShoulderDataIdx - $coarseShoulderDataIdx) points"
    Write-Host ""

    # Show detailed data around fine shoulder
    Write-Host "=== Data around FINE SHOULDER (+-50 points) ==="
    Write-Host "Time`tTurns`tTorque`tSmallWinDeriv"

    for($i=0; $i -lt $avgDerivativesSmall.Count; $i++) {
        $idx = $smallWindowCenters[$i]
        if ([Math]::Abs($idx - $fineShoulderDataIdx) -le 50) {
            $marker = if ($idx -eq $fineShoulderDataIdx) { " <-- SHOULDER" } else { "" }
            Write-Host "$([Math]::Round($times[$idx], 2))`t$([Math]::Round($turns[$idx], 4))`t$([Math]::Round($moments[$idx], 0))`t$([Math]::Round($avgDerivativesSmall[$i], 1))$marker"
        }
    }

    $workbook.Close($false)
}
finally {
    $excel.Quit()
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($worksheet) | Out-Null
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($workbook) | Out-Null
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}
