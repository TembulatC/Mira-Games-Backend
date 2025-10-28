# Development Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# Устанавливаем EF Core tools
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Копируем проект отдельно для лучшего кэширования
COPY MiraGamesBackend/*.csproj ./MiraGamesBackend/
RUN dotnet restore MiraGamesBackend/MiraGamesBackend.csproj

# Копируем всё
COPY . .

# Для разработки - не публикуем, а просто запускаем
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Запускаем с hot reload
CMD ["dotnet", "run", "--project", "MiraGamesBackend", "--urls", "http://0.0.0.0:8080"]