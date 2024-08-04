#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Dev2C2P.Services/Platform/Platform.API/Platform.API.csproj", "src/Dev2C2P.Services/Platform/Platform.API/"]
COPY ["src/Dev2C2P.Services/Platform/Platform.Application/Platform.Application.csproj", "src/Dev2C2P.Services/Platform/Platform.Application/"]
COPY ["src/Dev2C2P.Services/Platform/Platform.Contracts/Platform.Contracts.csproj", "src/Dev2C2P.Services/Platform/Platform.Contracts/"]
COPY ["src/Dev2C2P.Services/Platform/Platform.Domain/Platform.Domain.csproj", "src/Dev2C2P.Services/Platform/Platform.Domain/"]
COPY ["src/Dev2C2P.Services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj", "src/Dev2C2P.Services/Platform/Platform.Infrastructure/"]
RUN dotnet restore "src/Dev2C2P.Services/Platform/Platform.API/Platform.API.csproj"
COPY . .
WORKDIR "/src/src/Dev2C2P.Services/Platform/Platform.API"
RUN dotnet build "Platform.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Platform.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

RUN mkdir -p /app/tmp

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Platform.API.dll"]
