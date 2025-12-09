# ZTM Hub

ZTM Hub is a Vite + Vue 3 single-page app backed by an ASP.NET Core Web API. It lets authenticated users save their favorite Gdańsk ZTM stops, manage them (CRUD), and view live departure information pulled from the public ZTM feeds. Stops metadata is cached on the backend to avoid repeated heavy downloads, and a Leaflet map helps visualize stop locations.

## Tech choices (what this project actually uses)

- **Backend:** ASP.NET Core Web API (net10.0) with EF Core + SQLite, MediatR for use-cases, BCrypt.Net password hashing, JWT Bearer authentication, Swagger UI, HttpClient + IMemoryCache for ZTM data caching.
- **Frontend:** Vite + Vue 3 + TypeScript, Vue Router 4, Pinia 3 for auth/token storage, Axios with an interceptor that attaches the JWT, Tailwind CSS, a custom directive (`v-delay-color`), a custom plugin (`ztmPlugin`), reusable composables (e.g., `useZtmData`), and Vue-Leaflet for map visualization.
- **Testing:** Vitest + Vue Test Utils for unit/component tests, Playwright for E2E, and `dotnet test` for backend checks.
- **Architecture notes:** Single frontend (no micro-frontends). Backend serves as the API only; frontend is a separate Vite dev server.

## Getting started

### Prerequisites

- .NET SDK 10.0 (preview) for the backend.
- Node.js 20+ and npm for the frontend.

### Backend (API)

1. `cd backend`
2. Provide a strong JWT secret (user secrets or environment variable):
   - `dotnet user-secrets set "JwtSettings:Secret" "<your-secret>" --project src/ZtmHub.Api`
   - Alternatively, set `JwtSettings__Secret` in your environment.
3. Apply migrations to create the SQLite database:
   - `dotnet ef database update --project src/ZtmHub.Infrastructure --startup-project src/ZtmHub.Api`
4. Run the API:
   - `dotnet run --project src/ZtmHub.Api`
   - Default URLs: `http://localhost:5180` (matches the frontend Axios base URL) and Swagger at `/swagger`.
5. ZTM data sources can be adjusted in `appsettings.json` (`ZtmSettings:StopsUrl`, `DelaysBaseUrl`, `CacheDurationHours`). Stops metadata is cached in memory (default 24h).

### Frontend (Vite + Vue)

1. `cd frontend`
2. Install dependencies: `npm install`
3. Run dev server: `npm run dev`
4. The Axios client points to `http://localhost:5180/api` (see `src/api/axios.ts`). Update this URL if your API runs elsewhere.
5. Auth tokens are stored in `localStorage` by the Pinia auth store and attached automatically through the Axios interceptor.

## Running tests

- Backend: `cd backend && dotnet test`
- Frontend unit/component tests: `cd frontend && npm run test:run`
- Frontend E2E (Playwright): `cd frontend && npm run test:e2e`

## Key capabilities

- User registration and login with BCrypt-hashed passwords and JWT Bearer authentication.
- Swagger UI with Bearer auth to explore and try the API.
- CRUD for user-specific stops persisted via EF Core + SQLite value objects (login, password hash, stop identifiers).
- Live departures fetched from the ZTM API and combined with cached stop metadata.
- Vue Router–based navigation (login, register, dashboard) with route guards.
- Pinia-managed auth state and JWT propagation through Axios interceptors.
- Tailwind CSS–styled UI with reusable composables, a custom directive for delay coloring, a custom plugin, and Vue-Leaflet map widgets.

## Screenshots

Paste your captures here:

- ![Login view](./screenshots/login.png)
- ![Dashboard with departures](./screenshots/dashboard.png)
- ![Map of stops](./screenshots/map.png)
