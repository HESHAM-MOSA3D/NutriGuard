FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NutriGuard.Domain/NutriGuard.Domain.csproj", "NutriGuard.Domain/"]
COPY ["NutriGuard.Application/NutriGuard.Application.csproj", "NutriGuard.Application/"]
COPY ["NutriGuard.Infrastructure/NutriGuard.Infrastructure.csproj", "NutriGuard.Infrastructure/"]
COPY ["NutriGuard.API/NutriGuard.API.csproj", "NutriGuard.API/"]

RUN dotnet restore "NutriGuard.API/NutriGuard.API.csproj"

COPY . .
WORKDIR "/src/NutriGuard.API"
RUN dotnet publish "NutriGuard.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "NutriGuard.API.dll"]
