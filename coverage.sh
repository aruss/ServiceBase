#!/bin/bash

set -e

# https://github.com/OpenCover/opencover/pull/613
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.589 OpenCover -Source $PWD/tools
nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator

OPENCOVER=$PWD/packages/OpenCover.4.6.589/tools/OpenCover.Console.exe
REPORTGENERATOR=$PWD/packages/ReportGenerator.2.4.5.0/tools/ReportGenerator.exe
COVERAGE_DIR=./coverage/report
COVERAGE_HISTORY_DIR=./coverage/history

rm -rf $COVERAGE_DIR
mkdir -p $COVERAGE_DIR

PROJECTS=(\
"ServiceBase.UnitTests\ServiceBase.UnitTests.csproj")

for PROJECT in "${PROJECTS[@]}"
do
   :
$OPENCOVER \
  -target:"c:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test -f netcoreapp2.2 -c Release ./test/$PROJECT" \
  -mergeoutput \
  -hideskipped:File \
  -output:$COVERAGE_DIR/coverage.xml \
  -oldStyle \
  -filter:"+[ServiceBase*]* -[ServiceBase.*Tests*]*" \
  -searchdirs:./test/$PROJECT/bin/Release/netcoreapp2.2 \
  -register:user
done

echo "Generating HTML report"
$REPORTGENERATOR \
  -reports:$COVERAGE_DIR/coverage.xml \
  -targetdir:$COVERAGE_DIR \
  -historydir:$COVERAGE_HISTORY_DIR \
  -reporttypes:"Html" \
  -verbosity:Error


