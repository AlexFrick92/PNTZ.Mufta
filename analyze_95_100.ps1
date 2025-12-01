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

    Write-Host "=== Detailed Analysis 92-101 sec ==="
    Write-Host "Time`tTurns`tTorque`tWin50Spd`tWin200Spd"

    for($i=3900; $i -lt 4310; $i += 10) {
        if ($i -ge 200 -and $i -lt ($moments.Count - 200)) {
            # Window 50 points
            $sum50 = 0
            $count50 = 0
            for($j=($i-25); $j -lt ($i+25); $j++) {
                $dt = $times[$j+1] - $times[$j]
                if ($dt -gt 0) {
                    $sum50 += ($moments[$j+1] - $moments[$j]) / $dt
                    $count50++
                }
            }
            $avg50 = if ($count50 -gt 0) { $sum50 / $count50 } else { 0 }

            # Window 200 points
            $sum200 = 0
            $count200 = 0
            for($j=($i-100); $j -lt ($i+100); $j++) {
                $dt = $times[$j+1] - $times[$j]
                if ($dt -gt 0) {
                    $sum200 += ($moments[$j+1] - $moments[$j]) / $dt
                    $count200++
                }
            }
            $avg200 = if ($count200 -gt 0) { $sum200 / $count200 } else { 0 }

            Write-Host "$([Math]::Round($times[$i], 2))`t$([Math]::Round($turns[$i], 4))`t$([Math]::Round($moments[$i], 0))`t$([Math]::Round($avg50, 1))`t$([Math]::Round($avg200, 1))"
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
