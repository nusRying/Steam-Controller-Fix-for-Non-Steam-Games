Param(
    [string]$Configuration = 'Release',
    [string]$Runtime = 'win-x64',
    [string]$Output = '.\\publish'
)

Write-Host "Publishing SteamControllerBridge ($Configuration, $Runtime) to $Output"
dotnet publish --configuration $Configuration --runtime $Runtime /p:PublishSingleFile=true /p:PublishTrimmed=false /p:PublishReadyToRun=true -o $Output

# Copy dependencies we bundle (if present in build output)
$bin = Join-Path -Path (Get-Location) -ChildPath 'bin\\Debug\\net10.0-windows'
if (Test-Path $bin)
{
    Get-ChildItem -Path $bin -Filter 'SDL3.dll' -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $Output -Force
    }
    Get-ChildItem -Path $bin -Filter 'Nefarius.ViGEm.Client.dll' -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $Output -Force
    }
}

Write-Host "Publish complete. Output folder: $Output"
