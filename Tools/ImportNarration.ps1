param([string]$ProjectRoot)
Add-Type -AssemblyName System.Runtime.WindowsRuntime
$null=[Windows.Storage.StorageFile,Windows.Storage,ContentType=WindowsRuntime]
$null=[Windows.Storage.StorageFolder,Windows.Storage,ContentType=WindowsRuntime]
$null=[Windows.Media.Transcoding.MediaTranscoder,Windows.Media.Transcoding,ContentType=WindowsRuntime]
$null=[Windows.Media.MediaProperties.MediaEncodingProfile,Windows.Media.MediaProperties,ContentType=WindowsRuntime]
$null=[Windows.Media.Transcoding.PrepareTranscodeResult,Windows.Media.Transcoding,ContentType=WindowsRuntime]
$genericAwait=[System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object {$_.Name -eq 'AsTask' -and $_.IsGenericMethod -and $_.GetGenericArguments().Count -eq 1 -and $_.GetParameters().Count -eq 1} | Select-Object -First 1
$actionAwait=[System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object {$_.Name -eq 'AsTask' -and -not $_.IsGenericMethod -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncAction'} | Select-Object -First 1
function AwaitResult($operation,[Type]$resultType){$task=$genericAwait.MakeGenericMethod($resultType).Invoke($null,@($operation));$task.Wait();$task.Result}
$targetFolder=AwaitResult ([Windows.Storage.StorageFolder]::GetFolderFromPathAsync((Join-Path $ProjectRoot 'Assets\Resources\Varginha\Narration'))) ([Windows.Storage.StorageFolder])
$rows=@();$index=0
foreach($file in Get-ChildItem 'C:\Users\Usuario\Downloads' -Filter 'WhatsApp Audio 2026-10-29 at 10.*.mp4' | Sort-Object Name){
 $index++;$id='Narration{0:D2}' -f $index
 Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $ProjectRoot ('ArtSources\Narration20261029\'+$file.Name))
 $inputFile=AwaitResult ([Windows.Storage.StorageFile]::GetFileFromPathAsync($file.FullName)) ([Windows.Storage.StorageFile])
 $outputFile=AwaitResult ($targetFolder.CreateFileAsync($id+'.wav',1)) ([Windows.Storage.StorageFile])
 $profile=[Windows.Media.MediaProperties.MediaEncodingProfile]::CreateWav(2)
 $profile.Audio.SampleRate=16000;$profile.Audio.ChannelCount=1;$profile.Audio.BitsPerSample=16;$profile.Audio.Bitrate=256000
 $transcoder=New-Object Windows.Media.Transcoding.MediaTranscoder
 $prepared=AwaitResult ($transcoder.PrepareFileTranscodeAsync($inputFile,$outputFile,$profile)) ([Windows.Media.Transcoding.PrepareTranscodeResult])
 if(-not $prepared.CanTranscode){throw ('Cannot transcode '+$file.Name+': '+$prepared.FailureReason)}
 $task=$actionAwait.Invoke($null,@($prepared.TranscodeAsync()));$task.Wait()
 $rows+=@{id=$id;original=$file.Name;path=$outputFile.Path}
 Write-Output $id
}
$rows | ConvertTo-Json | Set-Content -Encoding UTF8 (Join-Path $ProjectRoot 'ArtSources\Narration20261029\manifest.json')
