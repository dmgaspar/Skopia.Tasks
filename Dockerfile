# =========================
# 1) Runtime (ASP.NET Core)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# =========================
# 2) Build (SDK)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj individuais (otimiza cache)
COPY ["Skopia.Tasks/Skopia.Tasks.csproj", "Skopia.Tasks/"]
COPY ["Application/Skopia.Tasks.Application.csproj", "Application/"]
COPY ["Skopia.Tasks.Domain/Skopia.Tasks.Domain.csproj", "Skopia.Tasks.Domain/"]
COPY ["Skopia.Tasks.Infrastructure/Skopia.Tasks.Infrastructure.csproj", "Skopia.Tasks.Infrastructure/"]

# Restore das dependências
RUN dotnet restore "Skopia.Tasks/Skopia.Tasks.csproj"

# Copiar toda a solução
COPY . .

# Build da aplicação
WORKDIR "/src/Skopia.Tasks"
RUN dotnet build "Skopia.Tasks.csproj" -c Release -o /app/build

# =========================
# 3) Publish (Release)
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
