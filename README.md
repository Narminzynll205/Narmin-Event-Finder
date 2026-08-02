# Event Finder — Backend

Event Finder — tədbir (event) təşkilatçılarının öz tədbirlərini paylaşdığı, iştirakçıların isə bu tədbirləri axtarıb tapa bildiyi platformadır.

## Əsas funksiyalar

1. **Tədbir paylaşma** — Organizer rolu yeni tədbir yarada bilir (ad, təsvir, kateqoriya, tarix/saat, ünvan, koordinatlar, şəkil, maks. iştirakçı sayı).
2. **Axtarış və baxış** — Participant rolu tədbirləri kateqoriya, tarix və məkana yaxınlığa görə axtara bilir.
3. **Xəritə sistemi** — Backend tədbirlərin lat/lng datasını qaytarır (frontend Leaflet/Google Maps ilə göstərəcək).
4. **Yaxınlıqdakı istifadəçilər** — Haversine formulu ilə cari yaxınlıqdakı digər istifadəçiləri tapmaq.
5. **Tədbirə qoşulma** — İstifadəçi "Join" edə bilir, iştirakçı siyahısına baxa bilir.
6. **Chat** — SignalR ilə real-time mesajlaşma.
7. **Autentifikasiya** — Qeydiyyat/Login, `Organizer` və `Participant` rolları (bir istifadəçi hər ikisini daşıya bilər).

## Texnologiyalar

- **Framework:** ASP.NET Core 8 MVC + Web API controller-ləri
- **Dil:** C#
- **ORM:** Entity Framework Core (Code-First, Migrations)
- **DB:** SQLite (development) / SQL Server (`appsettings.json`-da konfiqurasiya edilə bilər)
- **Auth:** ASP.NET Core Identity + JWT, rol əsaslı icazə
- **Real-time:** SignalR (Chat Hub)
- **Sənədləşdirmə:** Swagger / OpenAPI

## Arxitektura

```
EventFinder.Web/
 ├── Controllers/     # MVC + API controller-ləri (Auth, Events, Users, Chat, Home)
 ├── Models/          # Domain entity-ləri (ApplicationUser, Event, EventParticipant, ChatMessage...)
 ├── DTOs/            # Data Transfer Object-lər (Auth, Events, Users, Chat)
 ├── Data/            # DbContext, EF Core Migrations, seed data
 ├── Repositories/     # Data access qatı (Event, Chat)
 ├── Services/        # Biznes məntiqi (Auth, Event, User/Location, Chat)
 ├── Hubs/            # SignalR Hub-ları (ChatHub)
 ├── Middleware/       # Qlobal exception handling
 ├── Utils/           # Kömekçi funksiyalar (Haversine məsafə hesablama)
 └── Program.cs
```

## Quraşdırma

```bash
cd backend/EventFinder.Web
dotnet restore
dotnet ef database update
dotnet run
```

Layihə işə düşdükdən sonra Swagger UI: `https://localhost:<port>/swagger`

### Connection String

`appsettings.json` faylında `ConnectionStrings:DefaultConnection` dəyərini dəyişməklə SQLite və ya SQL Server arasında keçid etmək mümkündür.

## API Endpoint-ləri

```
POST   /api/auth/register
POST   /api/auth/login
GET    /api/events                     -> filter: category, dateFrom, dateTo, lat, lng, radiusKm
GET    /api/events/{id}
POST   /api/events                     -> [Authorize(Roles="Organizer")]
PUT    /api/events/{id}
DELETE /api/events/{id}
POST   /api/events/{id}/join
GET    /api/events/{id}/participants
GET    /api/users/nearby?lat=..&lng=..&radiusKm=..
PUT    /api/users/location
POST   /api/chat/send
GET    /api/chat/history/{id}?type=direct|event
/hubs/chat                              -> SignalR Hub (connect with ?access_token=<JWT>)
```

## Test istifadəçiləri (seed data)

Development mühitində avtomatik yaradılır:

| Email | Şifrə | Rollar |
|---|---|---|
| organizer@eventfinder.dev | Passw0rd! | Organizer, Participant |
| participant@eventfinder.dev | Passw0rd! | Participant |

## Frontend inteqrasiyası

Frontend ayrıca "Antigravity" aləti ilə hazırlanıb bu backend-ə qoşulacaq. Development mühitində CORS bütün origin-lərə açıqdır. **Production-da `Cors:AllowedOrigins` (`appsettings.json`) konfiqurasiyasına real frontend domeni yazılmalı və `FrontendPolicy` bu siyahı ilə məhdudlaşdırılmalıdır** (bax `Program.cs`).

## Xəta idarəetməsi

`/api/*` route-larında baş verən istisnalar `ExceptionHandlingMiddleware` tərəfindən tutulur və JSON formatında (status kodu + mesaj) qaytarılır. MVC (Razor) səhifələri isə standart `/Home/Error` səhifəsinə yönləndirilir.

## Qeydlər

- Şifrələr ASP.NET Core Identity-nin daxili hashing mexanizmi ilə saxlanılır, heç vaxt plain-text deyil.
- Bütün endpoint-lər üçün müvafiq HTTP status kodları qaytarılır (200, 201, 204, 400, 401, 403, 404).
- JWT konfiqurasiyası (`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryMinutes`) `appsettings.json`-dadır — **production-da `Jwt:Key` mütləq dəyişdirilməli və environment variable/secret manager vasitəsilə verilməlidir**.
