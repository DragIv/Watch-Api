# Crypto Watch API

ASP.NET Core REST API для управления криптовалютными портфелями с интеграцией реальных рыночных данных

## Описание проекта

**CryptoWatch API** - это демонстрационный проект, реализующий современные принципы разработки REST API на ASP.NET Core. Проект позволяет:

- **Управлять криптовалютными портфелями** (создание, редактирование, удаление)
- **Отслеживать криптовалютные позиции** (покупка/продажа активов)
- **Получать актуальные рыночные данные из CoinGecko API**
- **Демонстрировать лучшие практики ASP.NET Core разработки**

## Технологии и концепции

**Основной стек:**

- ASP.NET Core 8.0 - веб-фреймворк
- REST API - архитектурный стиль
- Swagger/OpenAPI - документация API
- Serilog - структурированное логирование
- In-Memory Repository - паттерн репозиторий
- Dependency Injection - встроенный DI контейнер

> [!NOTE]
> `In-Memory Repository` используется для демонстрационных целей. Для production-окружения рекомендуется использовать полноценную базу данных.

**Демонстрируемые концепции:**

- **REST API принципы:**
  - HTTP методы
  - Статус коды
  - Content negotiation: JSON формат
- **ASP.NET Core архитектура:**
  - Controller-based routing
  - Minimal APIs
  - Middleware Pipeline
  - Configuration System
  - Logging integration
- **Паттерны проектирования:**
  - Repository Pattern
  - Service Layer - бизнес-логика
  - DTO
  - Dependency Injection (IOC)

## Быстрый старт

**Предварительные требования:**
- .NET 8.0 SDK или выше
- IDE: Visual Studio, Rider, или VS Code

### Инструкции по запуску:
1.  **Клонировать репозиторий:**
    ```bash
    git clone https://github.com/DragIv/Watch-Api.git
    cd watch-api
    ```
2.  **Восстановить зависимости:**
    ```bash
    dotnet restore
    ```
3.  **Запустить приложение:**
    ```bash
    dotnet run
    ```

> [!TIP]
> После запуска приложения интерактивная документация API будет доступна по адресу `/swagger`.

## CryptoWatch API Documentation

### Portfolios (`/api/portfolios`)

#### Endpoints

| HTTP   | Endpoint               | Description            |
| :----- | :--------------------- | :--------------------- |
| GET    | `/api/portfolios`      | Получить все портфели  |
| GET    | `/api/portfolios/{id}` | Получить портфель по ID |
| POST   | `/api/portfolios`      | Создать новый портфель |
| PUT    | `/api/portfolios/{id}` | Обновить портфель      |
| DELETE | `/api/portfolios/{id}` | Удалить портфель       |

#### Работа с позициями

| HTTP   | Endpoint                                    | Description          |
| :----- | :------------------------------------------ | :------------------- |
| POST   | `/api/portfolios/{id}/holdings`             | Добавить криптовалюту |
| DELETE | `/api/portfolios/{id}/holdings/{holdingId}` | Продать криптовалюту  |

---

### Market Data (`/api/market`)

#### Endpoints

| HTTP | Endpoint                     | Description                    |
| :--- | :--------------------------- | :----------------------------- |
| GET  | `/api/market/quote/{symbol}` | Цена конкретной монеты         |
| GET  | `/api/market/top`            | Топ-20 монет по капитализации |

## Интеграция с внешними API

### CoinGecko API Integration

Проект демонстрирует работу с внешними REST API:

- **HTTP Client** — типизированный HttpClient через DI
- **Error Handling** — корректная обработка сетевых ошибок
- **JSON Deserialization** — работа с System.Text.Json
- **Rate Limiting** — учет ограничений внешнего API
- **Caching Strategy** — возможность добавления кеширования

---

## Возможные улучшения

Для production-ready версии можно добавить:

- [ ] Реальная база данных (Entity Framework Core + PostgreSQL/SQL Server)
- [ ] JWT Authentication вместо API ключей
- [ ] Application Insights или другой APM
- [ ] Rate Limiting для API endpoints
- [ ] Redis Caching для внешних API
- [ ] Unit & Integration Tests
- [ ] Docker containerization
- [ ] CI/CD pipeline

---

## Изученные концепции

Проект демонстрирует владение следующими технологиями и концепциями:

### ASP.NET Core Fundamentals

- Web API Controllers
- Minimal APIs
- Middleware Pipeline
- Dependency Injection
- Configuration System
- Logging Infrastructure

### REST API Design

- Resource-based URLs
- HTTP methods semantics
- Status codes usage
- Content negotiation
- Error handling
- API documentation

### Software Architecture

- Repository Pattern
- Service Layer
- DTO Pattern
- Separation of Concerns
- SOLID principles

### Modern Development Practices

- Structured logging
- Configuration management
- Error handling strategies
- API documentation
- HTTP client best practices
