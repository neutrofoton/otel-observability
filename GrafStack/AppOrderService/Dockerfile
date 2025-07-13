# ---------- Stage 1 : build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Salin & restore dependensi
COPY *.csproj .
RUN dotnet restore

# Salin seluruh source & publish
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Stage 2 : runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Gunakan variabel runtime port ASPNETCORE_URLS dari compose
ENTRYPOINT ["dotnet", "AppOrderService.dll"] 
