#!/bin/bash

set -e

# https://github.com/OpenCover/opencover/pull/613
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.589 OpenCover -Source $PWD/artifacts

OPENCOVER=$PWD/packages/OpenCover.4.6.589/tools/OpenCover.Console.exe
COVERAGE_DIR=./coverage

rm -rf $COVERAGE_DIR
mkdir $COVERAGE_DIR

PROJECTS=(\
"ServiceBase.UnitTests\ServiceBase.UnitTests.csproj")

for PROJECT in "${PROJECTS[@]}"
do
   :
$OPENCOVER \
  -target:"c:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test -f netcoreapp1.1 -c Release ./test/$PROJECT" \
  -mergeoutput \
  -hideskipped:File \
  -output:$COVERAGE_DIR/coverage.xml \
  -oldStyle \
  -filter:"+[ServiceBase*]* -[ServiceBase.*Tests*]*" \
  -searchdirs:./test/$PROJECT/bin/Release/netcoreapp1.1 \
  -register:user
done