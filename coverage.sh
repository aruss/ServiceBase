#!/bin/bash

set -e

# install report generator
nuget install -Verbosity quiet -OutputDirectory ./packages -Version 5.1.10 ReportGenerator

# run all tests
for f in ./test/**/*.csproj; do
dotnet test $f --no-build --collect:"XPlat Code Coverage"
done

# generate report
dotnet ./packages/ReportGenerator.5.1.10/tools/net6.0/ReportGenerator.dll \
    -reports:./test/*/TestResults/*/*.xml \
    -targetdir:./TestReport \
    -historydir:./TestReport/history

# remove all test reports
rm -rf ./test/**/TestResults
