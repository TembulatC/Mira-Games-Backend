# Development Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Копируем всё
COPY . .

# Для разработки - не публикуем, а просто запускаем
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080;https://+:8081

# Запускаем с hot reload
CMD ["dotnet", "run", "--project", "MiraGamesBackend", "--urls", "http://0.0.0.0:8080;https://0.0.0.0:8081"]