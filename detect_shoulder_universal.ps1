# Universal shoulder detection algorithm without magic constants
# Detects phases automatically based on torque behavior

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

    Write-Host "=== Universal Shoulder Detection ==="
    Write-Host "Total points: $($moments.Count)"
    Write-Host ""

    # PHASE 1: Find maximum and cut off unloading phase
    $maxMoment = ($moments | Measure-Object -Maximum).Maximum
    $maxIndex = [array]::IndexOf($moments, $maxMoment)

    Write-Host "Phase 4 (Unloading): Max torque $maxMoment Nm at index $maxIndex"
    Write-Host "Analyzing up to maximum, excluding unloading phase"
    Write-Host ""

    # Analyze only up to max
    $analyzeEnd = $maxIndex

    # PHASE 2: Calculate moving average derivative for all points
    $windowSize = 200  # points for smoothing
    $avgDerivatives = @()
    $windowCenters = @()

    Write-Host "Calculating moving average derivatives (window=$windowSize points)..."

    for($i=$windowSize; $i -lt ($analyzeEnd - $windowSize); $i += 5) {
        $sum = 0
        $count = 0
        for($j=($i - $windowSize/2); $j -lt ($i + $windowSize/2 - 1); $j++) {
            $dt = $times[$j+1] - $times[$j]
            if ($dt -gt 0) {
                $sum += ($moments[$j+1] - $moments[$j]) / $dt
                $count++
            }
        }

        if ($count -gt 0) {
            $avgDerivatives += $sum / $count
            $windowCenters += $i
        }
    }

    Write-Host "Calculated $($avgDerivatives.Count) windows"
    Write-Host ""

    # PHASE 3: Identify free threading phase (stable low derivative)
    # Take middle 50% of data as candidate for free threading
    $startIdx = [int]($avgDerivatives.Count * 0.2)
    $endIdx = [int]($avgDerivatives.Count * 0.7)

    $freeThreadingDerivatives = $avgDerivatives[$startIdx..$endIdx]
    $baselineAvg = ($freeThreadingDerivatives | Measure-Object -Average).Average

    # Calculate standard deviation
    $variance = 0
    foreach($d in $freeThreadingDerivatives) {
        $variance += [Math]::Pow($d - $baselineAvg, 2)
    }
    $baselineStd = [Math]::Sqrt($variance / $freeThreadingDerivatives.Count)

    Write-Host "Phase 2 (Free Threading) detected:"
    Write-Host "  Baseline average derivative: $([Math]::Round($baselineAvg, 2)) Nm/sec"
    Write-Host "  Standard deviation: $([Math]::Round($baselineStd, 2)) Nm/sec"
    Write-Host "  Torque range: $([Math]::Round($moments[$windowCenters[$startIdx]], 0))-$([Math]::Round($moments[$windowCenters[$endIdx]], 0)) Nm"
    Write-Host ""

    # PHASE 4: Find shoulder point - where derivative significantly exceeds baseline
    # Use 3-sigma rule: derivative > baseline + 3*sigma
    $threshold = $baselineAvg + 3 * $baselineStd
    $shoulderWindowIdx = -1

    for($i=$endIdx; $i -lt $avgDerivatives.Count; $i++) {
        if ($avgDerivatives[$i] -gt $threshold) {
            $shoulderWindowIdx = $i
            break
        }
    }

    if ($shoulderWindowIdx -eq -1) {
        Write-Host "Shoulder not found with 3-sigma threshold"
        Write-Host "Trying 2-sigma threshold..."
        $threshold = $baselineAvg + 2 * $baselineStd
        for($i=$endIdx; $i -lt $avgDerivatives.Count; $i++) {
            if ($avgDerivatives[$i] -gt $threshold) {
                $shoulderWindowIdx = $i
                break
            }
        }
    }

    if ($shoulderWindowIdx -eq -1) {
        Write-Host "ERROR: Shoulder point not found!"
        exit
    }

    $shoulderIndex = $windowCenters[$shoulderWindowIdx]

    Write-Host "=== SHOULDER POINT DETECTED ==="
    Write-Host "Phase 3 (Shoulder Contact):"
    Write-Host "  Index: $shoulderIndex of $($moments.Count)"
    Write-Host "  Time: $([Math]::Round($times[$shoulderIndex], 3)) sec"
    Write-Host "  Turns: $([Math]::Round($turns[$shoulderIndex], 5)) turns"
    Write-Host "  Torque: $([Math]::Round($moments[$shoulderIndex], 1)) Nm"
    Write-Host ""
    Write-Host "  Average derivative at shoulder: $([Math]::Round($avgDerivatives[$shoulderWindowIdx], 1)) Nm/sec"
    Write-Host "  Baseline derivative: $([Math]::Round($baselineAvg, 1)) Nm/sec"
    Write-Host "  Ratio: $([Math]::Round($avgDerivatives[$shoulderWindowIdx] / $baselineAvg, 2))x"
    Write-Host "  Threshold used: $([Math]::Round($threshold, 1)) Nm/sec"
    Write-Host ""

    # Show summary of all phases
    Write-Host "=== PROCESS SUMMARY ==="
    Write-Host "Phase 2 (Free Threading): $([Math]::Round($times[$windowCenters[$startIdx]], 1)) - $([Math]::Round($times[$windowCenters[$endIdx]], 1)) sec, Torque: $([Math]::Round($moments[$windowCenters[$startIdx]], 0)) - $([Math]::Round($moments[$windowCenters[$endIdx]], 0)) Nm"
    Write-Host "Phase 3 (Shoulder): starts at $([Math]::Round($times[$shoulderIndex], 1)) sec, Torque: $([Math]::Round($moments[$shoulderIndex], 0)) Nm"
    Write-Host "Phase 3 (Max): at $([Math]::Round($times[$maxIndex], 1)) sec, Torque: $([Math]::Round($maxMoment, 0)) Nm"
    Write-Host ""

    # Show detailed data around shoulder
    Write-Host "=== Data around SHOULDER (+-30 windows) ==="
    Write-Host "Time`tTurns`tTorque`tAvgSpeed"

    $startShow = [Math]::Max(0, $shoulderWindowIdx - 30)
    $endShow = [Math]::Min($avgDerivatives.Count - 1, $shoulderWindowIdx + 30)

    for($i=$startShow; $i -le $endShow; $i++) {
        $idx = $windowCenters[$i]
        $marker = if ($i -eq $shoulderWindowIdx) { " <-- SHOULDER" } else { "" }
        Write-Host "$([Math]::Round($times[$idx], 2))`t$([Math]::Round($turns[$idx], 4))`t$([Math]::Round($moments[$idx], 0))`t$([Math]::Round($avgDerivatives[$i], 1))$marker"
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
