FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["server/PortfolioCite.Api/PortfolioCite.Api.csproj", "server/PortfolioCite.Api/"]
COPY ["server/PortfolioCite.Application/PortfolioCite.Application.csproj", "server/PortfolioCite.Application/"]
COPY ["server/PortfolioCite.Domain/PortfolioCite.Domain.csproj", "server/PortfolioCite.Domain/"]
COPY ["server/PortfolioCite.Infrastructure/PortfolioCite.Infrastructure.csproj", "server/PortfolioCite.Infrastructure/"]
COPY ["PortfolioCite.Contracts/PortfolioCite.Contracts.csproj", "PortfolioCite.Contracts/"]

RUN dotnet restore "server/PortfolioCite.Api/PortfolioCite.Api.csproj"

COPY . .

RUN dotnet publish "server/PortfolioCite.Api/PortfolioCite.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortfolioCite.Api.dll"]
