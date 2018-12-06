dotnet restore ..\src\Spreads.Native\Spreads.Native.csproj
dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj --no-build -c RELEASE -o ..\..\artifacts

pause