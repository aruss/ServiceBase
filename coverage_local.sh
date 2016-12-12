#!/bin/bash

set -e

OPENCOVER=C:/opencover/OpenCover.Console.exe
REPORTGENERATOR=C:/opencover/tools/ReportGenerator.exe

CONFIG=Release
# Arguments to use for the build
DOTNET_BUILD_ARGS="-c $CONFIG"
# Arguments to use for the test
DOTNET_TEST_ARGS="$DOTNET_BUILD_ARGS"

echo CLI args: $DOTNET_BUILD_ARGS

#echo Restoring
#dotnet restore -v Warning

echo Building
dotnet build $DOTNET_BUILD_ARGS **/project.json

echo Testing

coverage=./coverage
rm -rf $coverage
mkdir $coverage

#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.EntityFramework.IntegrationTests
#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.EntityFramework.UnitTests
#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.Public.IntegrationTests
#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.Public.UnitTests
#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.UnitTests
#dotnet test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.UnitTests

echo "Calculating coverage with OpenCover"
$OPENCOVER \
  -target:"c:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test -f netcoreapp1.0 $DOTNET_TEST_ARGS test/ServiceBase.IdentityServer.Public.IntegrationTests" \
  -mergeoutput \
  -hideskipped:File \
  -output:$coverage/coverage.xml \
  -oldStyle \
  -filter:"+[ServiceBase*]* -[ServiceBase.*Tests*]*" \
  -searchdirs:$testdir/bin/$CONFIG/netcoreapp1.0 \
  -register:user
  
echo "Generating HTML report"
$REPORTGENERATOR \
  -reports:$coverage/coverage.xml \
  -targetdir:$coverage \
  -verbosity:Error
