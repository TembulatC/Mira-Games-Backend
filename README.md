# Steam Analytics Backend
### Быстрый запуск через Docker Compose
```
git clone https://github.com/TembulatC/Mira-Games-Backend.git
cd MiraGamesBackend
docker-compose up -d -build
```

### Сервисы будут доступны по адресам:
1. Основное API: http://localhost:5200
2. Swagger документация: http://localhost:5200/swagger
3. PostgreSQL: localhost:5434
4. ClickHouse: localhost:8124
5. Tabix UI: http://localhost:8125


### Архитектура
Проект построен по принципам чистой архитектуры с модульной структурой:

#### 1. Основные зоны:
1. `Domain` - зона где находится основной код модулей. У каждого модуля своя система слоев. Имя `Domain` ничего не значит, это просто указание, что там находится центральный код
2. `Infrastructure` - данный слой я вынес как отдельную зону. Там происходит реализация репозиториев модулей, регистрация базы данных в системе, миграции, скрипты инициализации и конфигурации таблиц
3. `Presentation` - основная зона для взаимодействия пользователя и приложения. Там регистрируются все зависимости, пространства имен, контроллеры, конфигурации, кэш JSON файлы и конфигурации

#### 2. Ключевые модули:
1. `SteamIntegration` - парсинг данных Steam и работа с API
2. `DataProcessing` - обработка и сохранение игровых данных
3. `Orchestrator` - Не имеет систему слоев. Все что он делает это координация процессов через Use Cases
4. `ClickHouse` - аналитика и статистика

#### 3. Каждый модуль в свою очередь разделен на слои - вот основные слои модулей:
1. `Models` - слой с доменными моделями данных. Есть в модулях - `DataProcessing`
2. `Interfaces` - слой с доменными интерфейсами для репозиториев, пайплайнов и сервисов. Есть в модулях - `SteamIntegration`, `DataProcessing`, `ClickHouse`
3. `Application` - слой со всей бизнес логикой. Там находятся уже сами сервисы, пайплайны и DTOs. Есть в модулях - `SteamIntegration`, `DataProcessing`, `ClickHouse`
4. `Infrastructure` - вынесен в отдельную внешнюю зону (см. выше). Относится к модулям - `SteamIntegration`, `DataProcessing`, `ClickHouse`


### Описание эндпоинтов:

#### 1. Управление данными:
1. `POST /Main/GetGames` - сбор данных об играх из Steam и запись их в JSON
2. `POST /Main/AddAndUpdateGamesData` - обновление данных в БД

#### 2. Календарь релизов
1. `GET /Main/games?month=YYYY-MM` - игры по месяцам
2. `GET /Main/games/calendar?month=YYYY-MM` - статистика по дням месяца
3. `GET /Main/games/filter?genre=...&supportPlatforms=...` - фильтрация игр

#### 3. Аналитика
1. `GET /Main/games/popularGenres?month=YYYY-MM` - популярные жанры
2. `POST /Main/statistics/addChangeDynamic` - создание снимка статистики
3. `GET /Main/statistics/getChangeDynamic?month=YYYY-MM` - динамика изменений


### Технологии:

#### 1. Backend:
1. ASP.NET Core 8
2. Entity Framework Core
3. HttpClient + HtmlAgilityPack

### 2. Базы данных:
1. PostgreSQL - основное хранение данных
2. ClickHouse - аналитика и статистика

### 3. Инфраструктура:
1. Docker + Docker Compose
2. Swagger/OpenAPI документация
3. Tabix - веб-интерфейс для ClickHouse


### Дополнительная информация
1. Поток данных происхрдит следующим образом - `Intefaces -> Repository -> Service/Pipeline -> UseCase -> Controller`
2. Некоторые UseCase и сервисы могут казаться избыточными из-за делегирования задач, что сделано преднамеренно для поддержания единого архитектурного стиля и обеспечения безопасного внедрения будущих функций
3. Все задачи выполняются в фоновом процессе, но для удобства в Swagger также реализованы эндпоинты для ручного управления приложением
