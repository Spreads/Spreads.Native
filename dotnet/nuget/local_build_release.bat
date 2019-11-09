@echo off

dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj -c Release -o \transient\LocalNuget --no-build -p:AutoSuffix=True

pause
