rm -rf ./publish
dotnet publish ./Plugins.WebHost/Plugins.WebHost.Dynamic.csproj -c Release -r win7-x64 -o ../publish/ 
::dotnet publish ./Plugins.PluginA/Plugins.PluginA.csproj -c Release -r win7-x64 -o ../publish/plugins/plugina 
dotnet publish ./Plugins.PluginB/Plugins.PluginB.csproj -c Release -r win7-x64 -o ../publish/plugins/pluginb 
