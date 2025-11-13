# =========================
# 1) Layer de runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# =========================
# 2) Build SDK
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj (otimiza cache)
COPY ["Skopia.Tasks/Skopia.Tasks.csproj", "Skopia.Tasks/"]
COPY ["Skopia.Tasks.Application/Skopia.Tasks.Application.csproj", "Skopia.Tasks.Application/"]
COPY ["Skopia.Tasks.Domain/Skopia.Tasks.Domain.csproj", "Skopia.Tasks.Domain/"]
COPY ["Skopia.Tasks.Infrastructure/Skopia.Tasks.Infrastructure.csproj", "Skopia.Tasks.Infrastructure/"]

RUN dotnet restore "Skopia.Tasks/Skopia.Tasks.csproj"

# Copiar tudo
COPY . .

WORKDIR "/src/Skopia.Tasks"
RUN dotnet build "Skopia.Tasks.csproj" -c Release -o /app/build

# =========================
# 3) Publish
# =========================
FROM build AS publish
RUN dotnet publish "Skopia.Tasks.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# 4) Imagem final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Skopia.Tasks.dll"]
