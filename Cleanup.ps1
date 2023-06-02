Get-ChildItem -Path . -Include bin,obj,artifacts -Recurse -Directory | Remove-Item -Recurse -Force
