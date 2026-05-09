FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Source/Kuva.Notifications.sln Source/
COPY Source/Kuva.Notifications.Entities/*.csproj Source/Kuva.Notifications.Entities/
COPY Source/Kuva.Notifications.Repository/*.csproj Source/Kuva.Notifications.Repository/
COPY Source/Kuva.Notifications.Business/*.csproj Source/Kuva.Notifications.Business/
COPY Source/Kuva.Notifications.Service/*.csproj Source/Kuva.Notifications.Service/
COPY Source/Kuva.Notifications.EFMigrations/*.csproj Source/Kuva.Notifications.EFMigrations/

RUN dotnet restore Source/Kuva.Notifications.Service/Kuva.Notifications.Service.csproj

COPY . .
WORKDIR /src/Source/Kuva.Notifications.Service
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Kuva.Notifications.Service.dll"]
