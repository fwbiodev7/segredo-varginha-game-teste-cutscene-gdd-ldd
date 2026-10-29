$ErrorActionPreference = 'Stop'
$taskDir = $PSScriptRoot
$taskStem = 'relatorio_desenvolvimento_o_segredo_de_varginha_23-10-2026'
$taskDocx = Join-Path $taskDir ($taskStem + '.docx')
$taskPdf = Join-Path $taskDir ($taskStem + '.pdf')
$taskWord = $null
$taskDoc = $null
try {
    $taskWord = New-Object -ComObject Word.Application
    $taskWord.Visible = $false
    $taskWord.DisplayAlerts = 0
    $taskDoc = $taskWord.Documents.Open($taskDocx, $false, $false)
    $taskDoc.Repaginate()
    $null = $taskDoc.Fields.Update()
    for ($taskIndex = 1; $taskIndex -le $taskDoc.TablesOfContents.Count; $taskIndex++) {
        $taskDoc.TablesOfContents.Item($taskIndex).Update()
    }
    foreach ($taskStyleId in @(-20, -21)) {
        $taskStyle = $taskDoc.Styles.Item($taskStyleId)
        $taskStyle.Font.Name = 'Times New Roman'
        $taskStyle.Font.Size = 12
        $taskStyle.Font.Color = 0
        $taskStyle.Font.Bold = $(if ($taskStyleId -eq -20) {-1} else {0})
        $taskStyle.ParagraphFormat.SpaceBefore = 0
        $taskStyle.ParagraphFormat.SpaceAfter = 0
        $taskStyle.ParagraphFormat.LineSpacingRule = 1
    }
    $taskDoc.Repaginate()
    $null = $taskDoc.Fields.Update()
    for ($taskIndex = 1; $taskIndex -le $taskDoc.TablesOfContents.Count; $taskIndex++) {
        $taskDoc.TablesOfContents.Item($taskIndex).UpdatePageNumbers()
    }
    $taskDoc.Save()
    $taskDoc.ExportAsFixedFormat($taskPdf, 17)
    [PSCustomObject]@{Pages=$taskDoc.ComputeStatistics(2);Words=$taskDoc.ComputeStatistics(0);TOC=$taskDoc.TablesOfContents.Count;PDF=$taskPdf} | ConvertTo-Json
}
finally {
    if ($null -ne $taskDoc) {$taskDoc.Close(0)}
    if ($null -ne $taskWord) {$taskWord.Quit()}
}
