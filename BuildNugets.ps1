# Set version
$VERSION = "6.0.2"

# Restore dependencies
dotnet restore ./ServiceBase.sln

# Build
dotnet build ./ServiceBase.sln --no-restore --configuration Release /property:Version=$VERSION

# Pack all the nugets
$csprojFiles = Get-ChildItem -Path .\src -Filter *.csproj -Recurse
foreach ($csprojFile in $csprojFiles) {
	$projectName = $csprojFile.BaseName
    nuget pack ./src/$projectName/$projectName.nuspec -OutputDirectory ./artifacts/packages -Properties Configuration=Release -version $VERSION -IncludeReferencedProjects
}