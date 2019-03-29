@echo off

dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj -c Release -o C:\transient\LocalNuget --no-build -p:AutoSuffix=True

pause
