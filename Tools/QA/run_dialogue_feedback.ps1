param([ValidateSet('EditMode','Player','Build')][string]$Mode='EditMode')
$ErrorActionPreference='Stop'
$projectPath=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$qaPath=Join-Path $projectPath 'output\dialogue-casual-2026-10-23'
$nodePath='C:\Users\Usuario\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe'
$unityPath='C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe'
$pythonPath='C:\Users\Usuario\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe'
if(Get-Process -Name Unity -ErrorAction SilentlyContinue){throw 'Há um Editor Unity aberto. Não iniciar uma segunda instância no projeto.'}
if(Test-Path -LiteralPath (Join-Path $projectPath 'Library\CampaignPolishRequest.json')){throw 'Há uma solicitação anterior do Editor pendente; não a consumir durante esta validação.'}
New-Item -ItemType Directory -Path $qaPath -Force | Out-Null
$backupPath=Join-Path $projectPath ('tmp\dialogue-casual-2026-10-23\backup-'+$Mode+'-'+(Get-Date -Format 'yyyyMMddHHmmss'))
New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
$settingsPath=Join-Path $projectPath 'ProjectSettings'
Get-ChildItem -LiteralPath $settingsPath -File | ForEach-Object {Copy-Item -LiteralPath $_.FullName -Destination $backupPath}
$urpPath=Join-Path $projectPath 'Assets\UniversalRenderPipelineGlobalSettings.asset'
Copy-Item -LiteralPath $urpPath -Destination (Join-Path $backupPath 'URP.original')
$settingsOriginal=[IO.File]::ReadAllText((Join-Path $settingsPath 'ProjectSettings.asset'))
$previousEvidence=$env:VARGINHA_CINEMA_QA
$commandExit=1
try {
    if ($Mode -ne 'Build') {
        # Keep QA saves and preferences separate while the released game is open.
        $isolated=$settingsOriginal -replace '(?m)^  productName:.*$', '  productName: Varginha_QA_Dialogue_20261023'
        [IO.File]::WriteAllText((Join-Path $settingsPath 'ProjectSettings.asset'),$isolated,[Text.UTF8Encoding]::new($false))
    }
    $env:VARGINHA_CINEMA_QA=$qaPath
    $unityArgs=@('-batchmode','-projectPath',$projectPath,'-logFile',(Join-Path $qaPath ($Mode+'.log')))
    if ($Mode -eq 'Build') {
        $unityArgs+=@('-quit','-executeMethod','Game.Editor.Testing.VarginhaDailyBuild.Build')
    } elseif ($Mode -eq 'EditMode') {
        $unityArgs+=@('-runTests','-testPlatform','EditMode','-testFilter','Game.Tests.EditMode.CampaignHouseInvestigationTests;Game.Tests.EditMode.CampaignMessagePresentationTests','-testResults',(Join-Path $qaPath 'editmode.xml'))
    } else {
        $unityArgs+=@('-runTests','-testPlatform','StandaloneWindows64','-testFilter','Game.Tests.PlayMode.CampaignChaptersTests.HouseDriveSchoolAndCodeFormAPlayableSavedCampaign;Game.Tests.PlayMode.CampaignMessagePlayTests.RenanSubtitleRevealsByKeyboardAndGamepadWithoutClosingEarly;Game.Tests.PlayMode.CampaignMessagePlayTests.RecordedVoiceUsesItsOwnVolumeAndPauseClock','-testResults',(Join-Path $qaPath 'player.xml'),'-playerHeartbeatTimeout','180')
    }
    & $nodePath (Join-Path $projectPath 'Tools\AgentOutput\compact.mjs') --question 'A revisão das falas compilou e concluiu os testes ou a build? Quais falhas?' -- $unityPath @unityArgs
    $commandExit=$LASTEXITCODE
} finally {
    Get-ChildItem -LiteralPath $backupPath -File | Where-Object {$_.Name -ne 'URP.original'} | ForEach-Object {Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $settingsPath $_.Name) -Force}
    Copy-Item -LiteralPath (Join-Path $backupPath 'URP.original') -Destination $urpPath -Force
    if($null -eq $previousEvidence){Remove-Item Env:VARGINHA_CINEMA_QA -ErrorAction SilentlyContinue}else{$env:VARGINHA_CINEMA_QA=$previousEvidence}
}
if($commandExit -eq 0 -and $Mode -ne 'Build'){
    $xmlPath=Join-Path $qaPath $(if($Mode -eq 'EditMode'){'editmode.xml'}else{'player.xml'})
    $jsonPath=Join-Path $qaPath $(if($Mode -eq 'EditMode'){'editmode-summary.json'}else{'player-summary.json'})
    & $pythonPath -X utf8 (Join-Path $projectPath 'Tools\QA\summarize_unity.py') $xmlPath --out $jsonPath
    $commandExit=$LASTEXITCODE
}
exit $commandExit
