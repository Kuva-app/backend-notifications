FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Source/Kuva.Email.sln Source/
COPY Source/Kuva.Email.Entities/*.csproj Source/Kuva.Email.Entities/
COPY Source/Kuva.Email.Repository/*.csproj Source/Kuva.Email.Repository/
COPY Source/Kuva.Email.Business/*.csproj Source/Kuva.Email.Business/
COPY Source/Kuva.Email.Service/*.csproj Source/Kuva.Email.Service/
COPY Source/Kuva.Email.EFMigrations/*.csproj Source/Kuva.Email.EFMigrations/

RUN dotnet restore Source/Kuva.Email.Service/Kuva.Email.Service.csproj

COPY . .
WORKDIR /src/Source/Kuva.Email.Service
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Kuva.Email.Service.dll"]
