$directory = Get-Location
$csprojFiles = Get-ChildItem -Path $directory -Filter *.csproj -Recurse

foreach ($csprojPath in $csprojFiles) {
    

    $nuspecPath = $csprojPath.FullName -replace '\.csproj$', '.nuspec'
    if (-not (Test-Path -Path $nuspecPath)) {
        Write-Host "No nuspec file found for $csprojPath, skipping..."
        continue
    }
	
	Write-Host "Updating nuspec file for: $csprojPath"

    [xml]$csprojFile = Get-Content -Path $csprojPath.FullName
    [xml]$nuspecFile = Get-Content -Path $nuspecPath

    $packageReferences = $csprojFile.Project.ItemGroup.PackageReference

    foreach ($packageReference in $packageReferences) {
        $id = $packageReference.Include
        $version = $packageReference.Version
        $nuspecDependency = $nuspecFile.package.metadata.dependencies.group.dependency | Where-Object { $_.id -eq $id }
        if ($null -ne $nuspecDependency) {
            $nuspecDependency.version = $version
        }
    }

    $nuspecFile.Save($nuspecPath)
}