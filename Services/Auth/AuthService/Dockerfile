# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier tout le projet
COPY . .

# Restaurer les dépendances
RUN dotnet restore

# Compiler et publier
RUN dotnet publish -c Release -o /app/publish

# ===== RUN =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copier les fichiers publiés
COPY --from=build /app/publish .

# Configuration ASP.NET
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Lancer l'application
ENTRYPOINT ["dotnet", "AuthService.dll"]
