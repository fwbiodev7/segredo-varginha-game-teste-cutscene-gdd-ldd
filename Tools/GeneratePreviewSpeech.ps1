$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Speech
$projectRoot = Split-Path -Parent $PSScriptRoot
$audioDir = Join-Path $projectRoot 'Assets\Resources\Varginha\Experiment\Audio'
New-Item -ItemType Directory -Path $audioDir -Force | Out-Null
$lines = @{
    NewsAnchor = 'Moradores de Varginha relatam o avistamento de uma criatura. As versões ainda se contradizem.'
    Witness = 'Eu vi alguma coisa perto da rua. Quando a luz voltou, já não estava lá.'
    Memory = 'Não deixa ela sair.'
}
foreach ($name in $lines.Keys) {
    $synthesizer = New-Object System.Speech.Synthesis.SpeechSynthesizer
    try {
        $synthesizer.SelectVoice('Microsoft Maria Desktop')
        $synthesizer.Rate = 0
        $path = Join-Path $audioDir ($name + '.wav')
        $synthesizer.SetOutputToWaveFile($path)
        $synthesizer.Speak($lines[$name])
        $synthesizer.SetOutputToNull()
        if ((Get-Item -LiteralPath $path).Length -le 44) { throw ('Empty audio: ' + $name) }
    } finally { $synthesizer.Dispose() }
}
Write-Output 'SYNTHESIZED_PT_BR_CLIPS=3'

