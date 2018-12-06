@echo off
dotnet test tests/Spreads.Native.Tests/Spreads.Native.Tests.csproj -c Release --no-build --filter TestCategory=CI -v n