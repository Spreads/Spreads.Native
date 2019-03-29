@echo off

dotnet restore ..\src\Spreads.Native\Spreads.Native.csproj
dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj -c DEBUG -o C:\transient\LocalNuget --no-build -p:AutoSuffix=True

pause