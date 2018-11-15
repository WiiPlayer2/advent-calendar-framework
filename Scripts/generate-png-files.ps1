$inkscapePath = "C:\Program Files\Inkscape\inkscape.exe"

Set-Location (Split-Path $PSScriptRoot -Parent)

$svgFiles = Get-ChildItem -Filter "*.svg" -Recurse

foreach($svgFile in $svgFiles)
{
    $pngFile = Join-Path $svgFile.Directory.FullName "$($svgFile.BaseName).png"
    Write-Output "Generating $pngFile";
    Start-Process $inkscapePath "-f", $svgFile.FullName, "-e", $pngFile -Wait
}
