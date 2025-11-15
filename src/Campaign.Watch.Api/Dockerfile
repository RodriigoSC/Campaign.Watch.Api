# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Campaign.Watch.Api/nuget.config", "src/Campaign.Watch.Api/"]
COPY ["src/Campaign.Watch.Api/Campaign.Watch.Api.csproj", "src/Campaign.Watch.Api/"]
COPY ["src/Campaign.Watch.Application/Campaign.Watch.Application.csproj", "src/Campaign.Watch.Application/"]
COPY ["src/Campaign.Watch.Domain/Campaign.Watch.Domain.csproj", "src/Campaign.Watch.Domain/"]
COPY ["src/Campaign.Watch.Infra.IoC/Campaign.Watch.Infra.IoC.csproj", "src/Campaign.Watch.Infra.IoC/"]
COPY ["src/Campaign.Watch.Infra.Data/Campaign.Watch.Infra.Data.csproj", "src/Campaign.Watch.Infra.Data/"]
RUN dotnet restore "./src/Campaign.Watch.Api/Campaign.Watch.Api.csproj"
COPY . .
WORKDIR "/src/src/Campaign.Watch.Api"
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