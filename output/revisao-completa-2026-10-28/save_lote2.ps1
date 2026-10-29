$ErrorActionPreference='Stop'
$originalIndex=$env:GIT_INDEX_FILE
try {
    $env:GIT_INDEX_FILE=Join-Path (Get-Location) 'output/revisao-completa-2026-10-28/checkpoint.index'
    git -c core.safecrlf=false add -- Assets/Scripts Assets/Tests Assets/Resources/Varginha/Revision20261028 Tools/QA/run_master_review.ps1 Docs/CampanhaOficial/PLANO_REVISAO_COMPLETA_2026-10-28.md
    if($LASTEXITCODE -ne 0){throw 'Falha ao guardar fontes no índice de checkpoint'}
    $tree=git write-tree
    $parent=(Get-Content output/revisao-completa-2026-10-28/checkpoint-lote1-commit.txt -Raw).Trim()
    $commit=git -c user.name=Codex -c user.email=codex@local commit-tree $tree -p $parent -m 'Oizlede revision batch 2: school, physical evidence, church, music and QA checkpoint'
    if($LASTEXITCODE -ne 0){throw 'Falha ao criar commit de checkpoint'}
    git update-ref refs/codex/checkpoints/revisao-20261028-lote2 $commit
    if($LASTEXITCODE -ne 0){throw 'Falha ao reter checkpoint'}
    $commit | Set-Content output/revisao-completa-2026-10-28/checkpoint-lote2-commit.txt
    Write-Output ('CHECKPOINT_LOTE2='+$commit)
} finally {$env:GIT_INDEX_FILE=$originalIndex}
