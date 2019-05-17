#!/bin/bash

set -e

# https://github.com/OpenCover/opencover/pull/613
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.589 OpenCover -Source $PWD/tools

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
  -targetargs:"test -f netcoreapp2.2 -c Release ./test/$PROJECT" \
  -mergeoutput \
  -hideskipped:File \
  -output:$COVERAGE_DIR/coverage.xml \
  -oldStyle \
  -filter:"+[ServiceBase*]* -[ServiceBase.*Tests*]*" \
  -searchdirs:./test/$PROJECT/bin/Release/netcoreapp2.2 \
  -register:user
done

if [ -n "$COVERALLS_REPO_TOKEN" ]
then
  nuget install -OutputDirectory packages -Version 0.7.0 coveralls.net
  packages/coveralls.net.0.7.0/tools/csmacnz.Coveralls.exe --opencover -i coverage/coverage.xml --useRelativePaths
fi