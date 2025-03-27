# Usa la imagen oficial de .NET SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código y compila
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen final para ejecutar
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "EmitCatTopic.dll"]
