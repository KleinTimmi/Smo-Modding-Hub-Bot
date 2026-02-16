# -------------------------
# 1️⃣ Build Stage
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Nur Projektordner kopieren
COPY SMO-Modding-Hub-Bot/*.csproj SMO-Modding-Hub-Bot/
RUN dotnet restore SMO-Modding-Hub-Bot/SMO-Modding-Hub-Bot.csproj

# Rest kopieren
COPY SMO-Modding-Hub-Bot/. SMO-Modding-Hub-Bot/
RUN dotnet publish SMO-Modding-Hub-Bot/SMO-Modding-Hub-Bot.csproj -c Release -o /app/publish

# -------------------------
# 2️⃣ Runtime Stage
# -------------------------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SMO_Modding_Hub_Bot.dll"]
