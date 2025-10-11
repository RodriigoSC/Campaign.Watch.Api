# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Campaign.Watch.Api/nuget.config", "Campaign.Watch.Api/"]
COPY ["Campaign.Watch.Api/Campaign.Watch.Api.csproj", "Campaign.Watch.Api/"]
COPY ["Campaign.Watch.Application/Campaign.Watch.Application.csproj", "Campaign.Watch.Application/"]
COPY ["Campaign.Watch.Domain/Campaign.Watch.Domain.csproj", "Campaign.Watch.Domain/"]
COPY ["Campaign.Watch.Infra.Campaign/Campaign.Watch.Infra.Campaign.csproj", "Campaign.Watch.Infra.Campaign/"]
COPY ["Campaign.Watch.Infra.Data/Campaign.Watch.Infra.Data.csproj", "Campaign.Watch.Infra.Data/"]
COPY ["Campaign.Watch.Infra.Effmail/Campaign.Watch.Infra.Effmail.csproj", "Campaign.Watch.Infra.Effmail/"]
COPY ["Campaign.Watch.Infra.Effpush/Campaign.Watch.Infra.Effpush.csproj", "Campaign.Watch.Infra.Effpush/"]
COPY ["Campaign.Watch.Infra.Effsms/Campaign.Watch.Infra.Effsms.csproj", "Campaign.Watch.Infra.Effsms/"]
COPY ["Campaign.Watch.Infra.Effwhatsapp/Campaign.Watch.Infra.Effwhatsapp.csproj", "Campaign.Watch.Infra.Effwhatsapp/"]
COPY ["Campaign.Watch.Infra.IoC/Campaign.Watch.Infra.IoC.csproj", "Campaign.Watch.Infra.IoC/"]
RUN dotnet restore "./Campaign.Watch.Api/Campaign.Watch.Api.csproj" --configfile ./Campaign.Watch.Api/nuget.config

COPY . .
WORKDIR "/src/Campaign.Watch.Api"
RUN dotnet build "./Campaign.Watch.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Campaign.Watch.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Campaign.Watch.Api.dll"]