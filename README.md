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

## Test edilib və işləkdir

Backend real HTTP sorğuları (curl-ekvivalent skriptlər) və real SignalR client ilə uçdan-uca test edilib. `dotnet build` (0 xəta/xəbərdarlıq), `dotnet ef database update` (SQLite DB yaradılıb, bütün migrasiyalar tətbiq olunub) və `dotnet run` (port açılıb, Swagger UI `/swagger` işləyir) təsdiqlənib.

Test edilmiş və işlək təsdiqlənmiş endpoint-lər:

- `POST /api/auth/register` — Organizer və Participant rolları ilə qeydiyyat
- `POST /api/auth/login` — hər iki rol üçün JWT token alınması
- `POST /api/events` — Organizer tərəfindən tədbir yaradılması (lat/lng ilə); Participant üçün 403 Forbidden düzgün qaytarılır
- `GET /api/events` — bütün tədbirlərin siyahısı
- `GET /api/events?category=&lat=&lng=&radiusKm=` — kateqoriya və məkana görə filtrasiya
- `GET /api/events/{id}` — tək tədbir
- `POST /api/events/{id}/join` — tədbirə qoşulma
- `GET /api/events/{id}/participants` — iştirakçı siyahısı
- `PUT /api/users/location` — istifadəçi məkanının yenilənməsi
- `GET /api/users/nearby` — 3+ fərqli koordinatlı test istifadəçisi ilə Haversine məsafə hesablamasının düzgünlüyü ədədi olaraq təsdiqlənib (radiusdan kənar istifadəçilər düzgün istisna edilir, nəticələr məsafəyə görə artan sırada)
- `POST /api/chat/send` və `GET /api/chat/history` — mesaj göndərilməsi və tarixçənin alınması
- `/hubs/chat` (SignalR) — real .NET SignalR client ilə iki qoşulma arasında canlı mesaj çatdırılması təsdiqlənib

**Edge case-lər:**

| Ssenari | Gözlənilən | Nəticə |
|---|---|---|
| Token olmadan qorunan endpoint-ə sorğu | 401 Unauthorized | ✅ Təsdiqləndi |
| Mövcud olmayan `eventId` | 404 Not Found | ✅ Təsdiqləndi |
| Maksimum iştirakçı sayına çatmış tədbirə qoşulma | 400 Bad Request + mesaj | ✅ Təsdiqləndi |
| Yanlış email/şifrə formatı ilə qeydiyyat | 400 Bad Request (validasiya) | ✅ Təsdiqləndi |

**Tapılan və düzəldilmiş xəta:** `ChatHub.SendMessage` metodu SignalR üzərindən göndərilən hər mesajı iki dəfə broadcast edirdi (həm `ChatService.SendMessageAsync` daxilində, həm də `ChatHub` daxilində təkrar) — nəticədə chat-də hər mesaj 2 dəfə görünürdü. Düzəliş: `ChatHub.SendMessage`-dən artıq broadcast məntiqi silindi, yalnız birbaşa mesajlarda göndərənin öz bağlantısına bildiriş saxlanıldı. Real SignalR client ilə düzəlişdən əvvəl (2 mesaj) və sonra (1 mesaj) test edilərək təsdiqləndi.
