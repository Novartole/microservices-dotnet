FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build-env

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim

WORKDIR /app

COPY --from=build-env /app/out ./

ENTRYPOINT ["dotnet", "PlatformService.dll"]