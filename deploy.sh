
#!/bin/bash
set -o allexport; source .env; set +o allexport

# Set version
VERSION=5.0.1

# Remove previous builds and artifacts
#find . -iname "bin" -o -iname "obj" -o -iname "artifacts" | xargs rm -rf

# Restore dependencies
#dotnet restore ./ServiceBase.sln

# Build
#dotnet build ./ServiceBase.sln --no-restore --configuration Release /property:Version=$VERSION

# Pack all the nugets
for PROJECT in "${PROJECTS[@]}"; do
nuget pack ./src/$PROJECT/$PROJECT.nuspec -OutputDirectory ./artifacts/packages -Properties Configuration=Release -version $VERSION -IncludeReferencedProjects
done

# Push all the nugets
for PROJECT in "${PROJECTS[@]}"; do
# nuget push ./artifacts/packages/$PROJECT.$VERSION.nupkg -SkipDuplicate -ApiKey $NUGET_API_KEY -Source https://api.nuget.org/v3/index.json
done
