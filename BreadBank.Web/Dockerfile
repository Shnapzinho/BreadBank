# Этап 1: Базовый образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Этап 2: Сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файл проекта из текущей папки (заметь, без приставки BreadBank.Web/)
COPY ["BreadBank.Web.csproj", "./"]
RUN dotnet restore "./BreadBank.Web.csproj"

# Копируем всё остальное содержимое папки
COPY . .

# Собираем проект
RUN dotnet build "BreadBank.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап 3: Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BreadBank.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этап 4: Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BreadBank.Web.dll"]