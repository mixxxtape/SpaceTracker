# Space Tracker — NASA Data Explorer

A web application for tracking near-Earth space objects, built with ASP.NET Core Web API, Entity Framework Core, and PostgreSQL. The app aggregates live data from multiple NASA and ISS public APIs behind a custom backend, and presents it through a dark, space-themed single-page frontend built in vanilla JavaScript.

---

## Screenshots

## Astronomy Picture of the Day
<img width="1917" height="857" alt="image" src="https://github.com/user-attachments/assets/83a741ec-7867-416d-9395-80068d135583" />


---

## Near-Earth Asteroids
<img width="1913" height="910" alt="image" src="https://github.com/user-attachments/assets/2e8dfbec-19ae-4fef-a18f-b42f47a6edeb" />


---

## ISS Live Tracking
<img width="1917" height="900" alt="image" src="https://github.com/user-attachments/assets/c44acab7-d826-4fb5-9268-3f164c541d7f" />


---

## Favorites Collection
<img width="1919" height="698" alt="image" src="https://github.com/user-attachments/assets/ff2e8faa-4ada-4195-bd0e-c87c9af410b4" />

---

## Features

### Guest
- Browse NASA's Astronomy Picture of the Day (APOD) for any date
- View near-Earth asteroid data with close-approach distance, speed, diameter, and a live countdown to closest approach
- Hazardous asteroids are visually flagged based on NASA's classification
- Track the International Space Station's real-time position on an interactive map
- Register or log in to unlock saving functionality

### Registered User
- **Favorites** — save any APOD photo to a personal collection; duplicate saves are prevented; toggle save/unsave directly from the photo view
- **Favorites History** — every add/delete action is logged with a timestamp for auditing
- Favorites are scoped per user — guests cannot save, and each user only sees their own collection

### Authentication & Security
- Registration and login with SHA-256 password hashing
- Email uniqueness validation on registration
- Stateless session handled client-side (no server session/cookies)

---

## Architecture

The backend follows a standard ASP.NET Core Web API structure:

```
SpaceTrackerAPIWebApp/
├── Controllers/   → ApodController, AsteroidsController, IssController, FavoritesController, AuthController
├── Services/      → NasaService, IssService (external API integration)
├── Models/        → User, Favorite, FavoriteHistory, SpaceTrackerContext
└── wwwroot/       → vanilla JS/HTML/CSS frontend (no framework)
```

Key design decisions:
- External NASA/ISS API responses are proxied through dedicated services and returned as raw JSON (`Content()`) to avoid double serialization
- All database timestamps use `DateTime.UtcNow` — PostgreSQL's `timestamp with time zone` requires UTC
- Favorite deletions are logged to a separate `FavoriteHistories` table rather than hard-deleted, preserving an audit trail
- ISS position track is rendered as a polyline split on antimeridian crossing (>180° longitude jump) to avoid drawing a line across the entire map

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 10.0 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL (Npgsql) |
| Frontend | Vanilla JavaScript, HTML, CSS (no framework) |
| Maps | Leaflet.js + OpenStreetMap |
| External APIs | NASA APOD, NASA NeoWs, wheretheiss.at |
| Containerization | Docker, Docker Compose (multi-stage build) |
| Testing | xUnit, EF Core InMemory provider |

---

## Testing

**API Testing (Postman)**
All endpoints were manually tested in Postman across the full CRUD lifecycle — GET, POST, DELETE. Exported collection: [`SpaceTracker.postman_collection.json`](./SpaceTracker.postman_collection.json)

19 unit tests covering `FavoritesController` and `AuthController` using xUnit and an in-memory EF Core database:
- Full CRUD coverage (GET/POST/PUT/DELETE) including not-found scenarios
- Registration validation (empty fields, duplicate email)
- Password hashing verification
- Login success/failure paths

```bash
dotnet test SpaceTrackerTests/SpaceTrackerTests.csproj
```

---

## Getting Started

1. Clone the repository
2. Set your NASA API key in `appsettings.json` (`DEMO_KEY` works with rate limits)
3. Run with Docker:
```bash
docker-compose up --build
```
4. Open `http://localhost:8080`

The PostgreSQL database and tables are created automatically on first startup.
