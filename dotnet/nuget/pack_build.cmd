del ..\artifacts\*.nupkg

dotnet restore ..\src\Spreads.Native\Spreads.Native.csproj
dotnet pack ..\src\Spreads.Native\Spreads.Native.csproj --no-build -c Release -o ..\artifacts  -p:AutoSuffix=True
xcopy /s ..\artifacts \transient\LocalNuget
pause