# Hotel Booking API

Лабораторна робота №2 — Розробка вебзастосунку на ASP.NET Core 8 Web API.

## Технології

- **ASP.NET Core 8 Web API**
- **Entity Framework Core 8** (Code First)
- **SQLite** (локальна розробка) / **SQL Server** (Production)
- **Swagger / OpenAPI**
- **HTML + Bootstrap 5 + JavaScript (Fetch API)**
- **Docker** (опціонально)

## Структура проєкту

```
HotelBookingApi/
├── Controllers/
│   ├── BookingsController.cs
│   ├── GuestsController.cs
│   ├── RoomsController.cs
│   └── RoomTypesController.cs
├── Data/
│   └── HotelContext.cs
├── DTOs/
│   ├── BookingCreateDto.cs
│   ├── BookingResponseDto.cs
│   ├── BookingUpdateDto.cs
│   ├── GuestCreateDto.cs
│   ├── GuestResponseDto.cs
│   ├── GuestUpdateDto.cs
│   ├── RoomCreateDto.cs
│   ├── RoomResponseDto.cs
│   ├── RoomTypeCreateDto.cs
│   ├── RoomTypeResponseDto.cs
│   ├── RoomTypeUpdateDto.cs
│   └── RoomUpdateDto.cs
├── Migrations/
├── Models/
│   ├── Booking.cs
│   ├── Guest.cs
│   ├── Room.cs
│   └── RoomType.cs
├── Services/
│   ├── BookingService.cs
│   ├── GuestService.cs
│   ├── RoomService.cs
│   ├── RoomTypeService.cs
│   └── ServiceResult.cs
├── wwwroot/
│   ├── css/site.css
│   ├── js/app.js
│   └── index.html
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── docker-compose.yml
└── Program.cs
```

## Моделі

| Модель | Поля |
|--------|------|
| **RoomType** | Id, Name, Capacity, PricePerNight |
| **Room** | Id, Number, Floor, IsAvailable, RoomTypeId |
| **Guest** | Id, FirstName, LastName, Email, Phone |
| **Booking** | Id, GuestId, RoomId, CheckIn, CheckOut, TotalPrice |

## API Endpoints

| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/roomtypes` | Список типів кімнат |
| GET | `/api/roomtypes/{id}` | Тип кімнати за Id |
| POST | `/api/roomtypes` | Створити тип кімнати |
| PUT | `/api/roomtypes/{id}` | Оновити тип кімнати |
| DELETE | `/api/roomtypes/{id}` | Видалити тип кімнати |
| GET | `/api/rooms` | Список кімнат |
| GET | `/api/rooms/{id}` | Кімната за Id |
| POST | `/api/rooms` | Створити кімнату |
| PUT | `/api/rooms/{id}` | Оновити кімнату |
| DELETE | `/api/rooms/{id}` | Видалити кімнату |
| GET | `/api/guests` | Список гостей |
| GET | `/api/guests/{id}` | Гість за Id |
| POST | `/api/guests` | Створити гостя |
| PUT | `/api/guests/{id}` | Оновити гостя |
| DELETE | `/api/guests/{id}` | Видалити гостя |
| GET | `/api/bookings` | Список бронювань |
| GET | `/api/bookings/{id}` | Бронювання за Id |
| POST | `/api/bookings` | Створити бронювання |
| PUT | `/api/bookings/{id}` | Оновити бронювання |
| DELETE | `/api/bookings/{id}` | Видалити бронювання |

## Бізнес-логіка

### Booking
- `CheckOut` має бути пізніше за `CheckIn` → **400 Bad Request**
- Room та Guest повинні існувати → **400 Bad Request**
- Якщо кімната вже заброньована на вибраний період → **409 Conflict**
- `TotalPrice` обчислюється автоматично: кількість ночей × `PricePerNight`

### Room
- Номер кімнати (`Number`) повинен бути унікальним → **409 Conflict**
- Поверх (`Floor`) має бути ≥ 1

### Guest
- `Email` повинен бути унікальним → **409 Conflict**
- `Email` перевіряється атрибутом `[EmailAddress]`
- `Phone` не може бути порожнім

## Запуск локально

```bash
cd HotelBookingApi
dotnet run --urls "http://localhost:5050"
```

Після запуску:
- **Фронтенд:** http://localhost:5050
- **Swagger UI:** http://localhost:5050/swagger

> База даних SQLite (`hotel.db`) та міграції застосовуються автоматично при першому запуску.

## Запуск через Docker

```bash
docker compose up --build
```

- **Фронтенд / API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

> SQL Server запускається автоматично у контейнері.

## HTTP Status Codes

| Код | Значення |
|-----|---------|
| 200 | OK |
| 201 | Created |
| 204 | No Content (delete) |
| 400 | Bad Request (невалідні дані) |
| 404 | Not Found |
| 409 | Conflict (дублювання, накладення бронювань) |
