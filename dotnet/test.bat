@echo off
dotnet test tests/Spreads.Native.Tests.NuGet/Spreads.Native.Tests.NuGet.csproj -c Release --filter TestCategory=CI -v n -- RunConfiguration.TargetPlatform=x64