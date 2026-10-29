param(
    [ValidateSet('Compile','EditMode','PlayMode','Player','Build','References')][string]$Mode='EditMode',
    [string]$Filter='',
    [string]$Name='review',
    [switch]$SkipSync,
    [string]$EvidencePath='output/revisao-final-2026-10-26'
)
$ErrorActionPreference='Stop'
$projectPath=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$qaProject=Join-Path $env:TEMP 'varginha-master-qa-20261023-120709'
$evidence=Join-Path $projectPath $EvidencePath
$unity='C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Unity.exe'
if(!(Test-Path -LiteralPath (Join-Path $qaProject 'Library'))){throw 'A cópia isolada de QA não foi encontrada.'}
$running=Get-CimInstance Win32_Process -Filter "name='Unity.exe'" | Where-Object {$_.CommandLine -like ('*'+$qaProject+'*')}
if($running){throw 'Já existe Unity usando a cópia de QA. Aguarde o término antes de sincronizar.'}
New-Item -ItemType Directory -Path $evidence -Force | Out-Null
if(!$SkipSync){
    foreach($folder in @('Assets','Packages','ProjectSettings')){
        & robocopy (Join-Path $projectPath $folder) (Join-Path $qaProject $folder) /E /R:1 /W:1 /NFL /NDL /NJH /NJS /NP /XD _Recovery /XF 'InitTestScene*.unity*' *> (Join-Path $evidence ('sync-'+$folder+'.log'))
        if($LASTEXITCODE -ge 8){throw ('Falha ao sincronizar '+$folder)}
    }
    # The user's working URP file may contain temporary runtime changes.
    $urp=& git -C $projectPath show HEAD:Assets/UniversalRenderPipelineGlobalSettings.asset
    if($LASTEXITCODE -ne 0){throw 'Falha ao recuperar a configuração URP versionada.'}
    [IO.File]::WriteAllText((Join-Path $qaProject 'Assets/UniversalRenderPipelineGlobalSettings.asset'),($urp -join "`n"),[Text.UTF8Encoding]::new($false))
}
$settings=Join-Path $qaProject 'ProjectSettings/ProjectSettings.asset'
$text=[IO.File]::ReadAllText($settings)
if($Mode -ne 'Build'){$text=$text -replace '(?m)^  productName:.*$','  productName: Varginha_Completion_QA_20261027'}
[IO.File]::WriteAllText($settings,$text,[Text.UTF8Encoding]::new($false))
$env:VARGINHA_CINEMA_QA=$evidence
$env:VARGINHA_FINAL_BUILD_ROOT=$projectPath
$argsUnity=@('-batchmode','-force-d3d11','-projectPath',$qaProject,'-logFile',(Join-Path $evidence ($Name+'.log')))
if($Mode -eq 'Build'){$argsUnity+=@('-nographics','-quit','-executeMethod','Game.Editor.Testing.VarginhaDailyBuild.Build')}
elseif($Mode -eq 'References'){$argsUnity+=@('-quit','-executeMethod','Game.Editor.Testing.CampaignDirectionalReferences.Export')}
elseif($Mode -eq 'Compile'){$argsUnity+=@('-quit')}
else{
    $platform=if($Mode -eq 'Player'){'StandaloneWindows64'}else{$Mode}
    $argsUnity+=@('-runTests','-testPlatform',$platform,'-testResults',(Join-Path $evidence ($Name+'.xml')))
    if($Filter){$argsUnity+=@('-testFilter',$Filter)}
    # The full world audit has a 600s NUnit deadline. Keep the connection alive
    # until NUnit can report its own result; do not replace it with a socket timeout.
    if($Mode -eq 'Player'){$argsUnity+=@('-playerHeartbeatTimeout','720')}
}
& node (Join-Path $projectPath 'Tools/AgentOutput/compact.mjs') --question 'A revisão final compilou e executou todos os testes selecionados ou a build? Informe falhas e contagens.' -- $unity @argsUnity
$reviewExit=$LASTEXITCODE
if($reviewExit -eq 0 -and $Mode -ne 'Build' -and $Mode -ne 'References' -and $Mode -ne 'Compile'){
    & python (Join-Path $projectPath 'Tools/QA/summarize_unity.py') (Join-Path $evidence ($Name+'.xml')) --out (Join-Path $evidence ($Name+'-summary.json'))
    $reviewExit=$LASTEXITCODE
}
exit $reviewExit
