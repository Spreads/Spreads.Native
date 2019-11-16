FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY tests/Spreads.Native.Run/Spreads.Native.Run.csproj tests/Spreads.Native.Run/
COPY tests/Spreads.Native.Tests/Spreads.Native.Tests.csproj tests/Spreads.Native.Tests/
COPY src/Spreads.Native/Spreads.Native.csproj src/Spreads.Native/
RUN dotnet restore tests/Spreads.Native.Run/Spreads.Native.Run.csproj
COPY . .
WORKDIR /src/tests/Spreads.Native.Run
RUN dotnet build Spreads.Native.Run.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Spreads.Native.Run.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Spreads.Native.Run.dll"]
