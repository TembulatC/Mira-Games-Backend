FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /app

# Установка EF Core 
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Переменные окружения
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Копируем solution/project файлы
COPY ["*.sln", "."]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["MiraGamesBackend/MiraGamesBackend.csproj", "MiraGamesBackend/"]

RUN dotnet restore "MiraGamesBackend/MiraGamesBackend.csproj"

# Копируем весь код
COPY . .

# Собираем
RUN dotnet build "MiraGamesBackend/MiraGamesBackend.csproj" -c Release -o /app/build

# Публикуем
RUN dotnet publish "MiraGamesBackend/MiraGamesBackend.csproj" -c Release -o /app/publish

# Открываем порт
EXPOSE 8080

# Запускаем приложение
CMD ["dotnet", "run", "--project", "MiraGamesBackend/MiraGamesBackend.csproj", "--urls", "http://0.0.0.0:8080"]