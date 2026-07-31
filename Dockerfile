# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NutriGuard-master/NutriGuard.API/NutriGuard.API.csproj", "NutriGuard.API/"]
COPY ["NutriGuard-master/NutriGuard.Application/NutriGuard.Application.csproj", "NutriGuard.Application/"]
COPY ["NutriGuard-master/NutriGuard.Domain/NutriGuard.Domain.csproj", "NutriGuard.Domain/"]
COPY ["NutriGuard-master/NutriGuard.Infrastructure/NutriGuard.Infrastructure.csproj", "NutriGuard.Infrastructure/"]

RUN dotnet restore "NutriGuard.API/NutriGuard.API.csproj"

COPY ./NutriGuard-master .
WORKDIR "/src/NutriGuard.API"
RUN dotnet publish "NutriGuard.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NutriGuard.API.dll"]
