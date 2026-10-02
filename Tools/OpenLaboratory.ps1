$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
if (Test-Path 'V:\ProjectSettings\ProjectVersion.txt') {
    $mapping = & subst
    if (-not ($mapping | Where-Object { $_ -like ('V:*' + $projectRoot + '*') })) {
        throw 'A unidade V: está em uso por outro projeto. Abra esta cópia por um caminho curto.'
    }
} else { & subst V: $projectRoot; if ($LASTEXITCODE -ne 0) { throw 'Não foi possível criar o caminho curto.' } }
& unity open 'V:\' --editor-version 6000.6.0f1 --args '-executeMethod Game.Editor.Testing.VarginhaExperimentBuilder.Open'
