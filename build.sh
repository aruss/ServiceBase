#!/bin/bash

dotnet --info
dotnet restore --no-cache
dotnet build .\ServiceBase.sln --configuration Release