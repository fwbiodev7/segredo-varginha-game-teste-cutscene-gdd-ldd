$ErrorActionPreference='Stop'
$previousIndexVariable=$env:GIT_INDEX_FILE
try {
    $env:GIT_INDEX_FILE=Join-Path (Get-Location) 'output/revisao-completa-2026-10-28/checkpoint.index'
    git -c core.safecrlf=false add -- Assets/Scripts/Game/Varginha/Experiment/CampaignHouseInvestigation.cs Assets/Scripts/Game/Varginha/Experiment/CampaignHouseSequence.cs Assets/Scripts/Game/Varginha/Experiment/VarginhaCampaignStage.cs Assets/Scripts/Game/Varginha/VarginhaMainMenu.cs Assets/Scripts/Game/Varginha/Experiment/CampaignGuidance.cs Assets/Scripts/Game/Varginha/Experiment/CampaignPuzzleDesign.cs Assets/Tests/EditMode/CampaignHouseInvestigationTests.cs Assets/Tests/PlayMode/CampaignMessagePlayTests.cs Assets/Tests/PlayMode/CampaignPuzzleDesignPlayTests.cs Assets/Resources/Varginha/Revision20261028 Assets/Resources/Varginha/Revision20261028.meta Docs/CampanhaOficial/PLANO_REVISAO_COMPLETA_2026-10-28.md
    if($LASTEXITCODE -ne 0){throw 'Failed to stage checkpoint'}
    $tree=git write-tree
    $parent=(Get-Content output/revisao-completa-2026-10-28/checkpoint-commit.txt -Raw).Trim()
    $commit=git -c user.name=Codex -c user.email=codex@local commit-tree $tree -p $parent -m 'Oizlede approved revision batch 1 checkpoint'
    if($LASTEXITCODE -ne 0){throw 'Failed to commit checkpoint'}
    git update-ref refs/codex/checkpoints/revisao-20261028-lote1 $commit
    if($LASTEXITCODE -ne 0){throw 'Failed to retain checkpoint'}
    $commit | Set-Content output/revisao-completa-2026-10-28/checkpoint-lote1-commit.txt
    Write-Output ('CHECKPOINT_LOTE1='+$commit)
} finally {$env:GIT_INDEX_FILE=$previousIndexVariable}
