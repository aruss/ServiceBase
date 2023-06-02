# Stop script execution on error
$ErrorActionPreference = "Stop"

# Install report generator
Write-Host "Installing ReportGenerator..."
nuget install -Verbosity quiet -OutputDirectory ./packages -Version 5.1.10 ReportGenerator

# Run all tests
$testProjects = Get-ChildItem -Path ./test -Filter *.csproj -Recurse
foreach ($testProject in $testProjects) {
    Write-Host "Running tests on project $testProject..."
    dotnet test $testProject.FullName --no-build --collect:"XPlat Code Coverage"
}

# Generate report
Write-Host "Generating report..."
dotnet ./packages/ReportGenerator.5.1.10/tools/net6.0/ReportGenerator.dll `
    -reports:./test/*/TestResults/*/*.xml `
    -targetdir:./TestReport `
    -historydir:./TestReport/history

# Remove all test reports
Write-Host "Removing test reports..."
Get-ChildItem -Path ./test -Filter TestResults -Recurse -Directory | Remove-Item -Recurse -Force
Write-Host "Done."