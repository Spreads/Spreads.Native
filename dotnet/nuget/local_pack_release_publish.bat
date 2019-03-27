@echo off
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"

REM set "datestamp=%YYYY%%MM%%DD%" & set "timestamp=%HH%%Min%%Sec%"
set "fullstamp=%YY%%MM%%DD%%HH%%Min%"
REM echo datestamp: "%datestamp%"
REM echo timestamp: "%timestamp%"
REM echo fullstamp: "%fullstamp%"

set "build=build%fullstamp%"
echo build: "%build%"

dotnet test ..\tests\Spreads.Native.Tests\Spreads.Native.Tests.csproj -c RELEASE --no-build --filter TestCategory=CI -v n

dotnet restore ..\src\Spreads.Native\Spreads.Native.csproj
dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj -c RELEASE -o C:\transient\LocalNuget --no-build --version-suffix "%build%"R

@for %%f in (C:\transient\LocalNuget\*"%build%"R.nupkg) do @C:\tools\nuget\NuGet.exe push %%f -source https://www.nuget.org/api/v2/package

pause