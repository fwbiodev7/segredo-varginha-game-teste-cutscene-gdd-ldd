param([ValidateSet('EditMode','Player','Build')][string]$Mode='EditMode')
$ErrorActionPreference='Stop'
$projectPath=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$qaPath=Join-Path $projectPath 'output\school-desk-2026-10-22'
$nodePath='C:\Users\Usuario\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe'
$unityPath='C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe'
$settingsPath=Join-Path $projectPath 'ProjectSettings'
$backupPath=Join-Path $qaPath ('backup-'+$Mode+'-'+(Get-Date -Format 'yyyyMMddHHmmss'))
New-Item -ItemType Directory -Path $backupPath | Out-Null
Get-ChildItem -LiteralPath $settingsPath -File | ForEach-Object {Copy-Item -LiteralPath $_.FullName -Destination $backupPath}
$urpPath=Join-Path $projectPath 'Assets\UniversalRenderPipelineGlobalSettings.asset'
Copy-Item -LiteralPath $urpPath -Destination (Join-Path $backupPath 'URP.original')
$settingsOriginal=[IO.File]::ReadAllText((Join-Path $settingsPath 'ProjectSettings.asset'))
try {
    if ($Mode -ne 'Build') {
        # Product separation gives the QA Editor/Player its own persistentDataPath
        # and registry hive while the user's released game can remain open.
        $isolated=$settingsOriginal -replace '(?m)^  productName:.*$', '  productName: Varginha_QA_SchoolFeedback_20261022'
        [IO.File]::WriteAllText((Join-Path $settingsPath 'ProjectSettings.asset'),$isolated,[Text.UTF8Encoding]::new($false))
    }
    $env:VARGINHA_CINEMA_QA=$qaPath
    $unityArgs=@('-batchmode','-projectPath',$projectPath,'-logFile',(Join-Path $qaPath ($Mode+'.log')))
    if ($Mode -eq 'Build') {$unityArgs+=@('-quit','-executeMethod','Game.Editor.Testing.VarginhaDailyBuild.Build')}
    elseif ($Mode -eq 'EditMode') {$unityArgs+=@('-runTests','-testPlatform','EditMode','-testResults',(Join-Path $qaPath 'editmode.xml'))}
    else {$unityArgs+=@('-runTests','-testPlatform','StandaloneWindows64','-testFilter','Game.Tests.PlayMode.CampaignSchoolDeskPlayTests;Game.Tests.PlayMode.CampaignCinematicOpeningPlayTests.AdultHouseHasDaylightGlassWithoutChangingFurnitureOrProgress','-testResults',(Join-Path $qaPath 'player.xml'),'-playerHeartbeatTimeout','180')}
    & $nodePath (Join-Path $projectPath 'Tools\AgentOutput\compact.mjs') --question 'Compilou e terminou? Quantos testes e quais falhas?' -- $unityPath @unityArgs
    $commandExit=$LASTEXITCODE
}
finally {
    Get-ChildItem -LiteralPath $backupPath -File | Where-Object {$_.Name -ne 'URP.original'} | ForEach-Object {Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $settingsPath $_.Name) -Force}
    Copy-Item -LiteralPath (Join-Path $backupPath 'URP.original') -Destination $urpPath -Force
    Remove-Item Env:VARGINHA_CINEMA_QA -ErrorAction SilentlyContinue
}
exit $commandExit
