param (
    [string] $PackageDir = 'out',
    
    [Parameter(Mandatory)]
    [string] $Version 
)

$projectFile = "$PSScriptRoot/src/DartSassBuilder/DartSassBuilder.csproj"
$nuspecProjectFile = "$PSScriptRoot/package/DartSassBuilder.csproj"
$nuspecFile = "$PSScriptRoot/package/DartSassBuilder.nuspec"

& dotnet build $projectFile -p:Version=$Version -c Release -o "$PSScriptRoot/package/tool"
& dotnet pack $projectFile -p:Version=$Version -c Release -o $PackageDir
$xml = [Xml](Get-Content -Path $nuspecFile -Raw)
$xml.package.metadata.version = $Version
$xml.Save($nuspecFile)
& dotnet pack $nuspecProjectFile -p:Version=$Version -c Release -o $PackageDir
#& dotnet pack --project $projectFile -pVersion=$Version -o $OutputDir