Param(
    [Parameter(Mandatory=$false)]
    [string]$SdlZipUrl
)

function Ensure-Dir($path){ if(-not (Test-Path $path)){ New-Item -ItemType Directory -Path $path | Out-Null } }

$outDir = Join-Path -Path (Get-Location) -ChildPath "bin\Debug\net10.0-windows"
Ensure-Dir $outDir

if(-not $SdlZipUrl){
    Write-Host "No SDL zip URL provided. Pass a direct download URL to a Windows SDL3 redistributable zip as the first argument." -ForegroundColor Yellow
    Write-Host "Example: .\install-deps.ps1 -SdlZipUrl 'https://example.com/SDL3-3.0.0-win64.zip'"
    exit 1
}

$tmp = Join-Path $env:TEMP ([IO.Path]::GetRandomFileName())
Ensure-Dir $tmp
$zip = Join-Path $tmp "sdl.zip"

Write-Host "Downloading $SdlZipUrl ..."
Invoke-WebRequest -Uri $SdlZipUrl -OutFile $zip

Write-Host "Extracting..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zip, $tmp)

$dlls = Get-ChildItem -Path $tmp -Recurse -Filter SDL3.dll -ErrorAction SilentlyContinue
if($dlls.Count -eq 0){
    Write-Host "SDL3.dll not found in the archive." -ForegroundColor Red
    exit 1
}

foreach($dll in $dlls){ Copy-Item -Path $dll.FullName -Destination $outDir -Force }

Write-Host "Copied SDL3.dll to $outDir"
Write-Host "Cleanup..."
Remove-Item -Path $tmp -Recurse -Force

Write-Host "Done. You can now run: dotnet run --project . run"
