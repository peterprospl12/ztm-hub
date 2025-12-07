# Zadanie domowe z Vue.js/React + backend (.NET/Express)

W ramach otwartych danych ZTM w Gdańsku mamy następujące API:

- https://ckan.multimediagdansk.pl/dataset/c24aa637-3619-4dc2-a171-a23eec8f2172/resource/4c4025f0-01bf-41f7-a39f-d156d201b82b/download/stops.json – lista przystanków, zawiera stopId – identyfikator słupka przystankowego unikalny w skali Trójmiasta.
- http://ckan2.multimediagdansk.pl/delays?stopId={stopId} – estymowane czasy przyjazdów na przystanek, gdzie {stopId} jest identyfikatorem słupka – wartość stopId z zasobu Lista przystanków. Przykładowe zapytanie dla przystanku na Miszewskiego (stopId=2019): http://ckan2.multimediagdansk.pl/delays?stopId=2019. Dostajemy kolekcję pojazdów które pojawią się na przystanku w najbliższym okresie. Dane są zapisane z użyciem formatu JSON. Mamy tam kilka właściwości zawierających interesujące aktualne informacje, takie jak: numer linii:"routeId", opóźnienie:"delayInSeconds", czas przyjazdu wg rozkładu: "theoreticalTime", rzeczywisty czas odjazdu: "estimatedTime". Dane można je porównać z wybrana podstroną ZTM: https://mapa.ztm.gda.pl/?stop=2019.

```json
{
  "lastUpdate": "2024-11-16 19:09:13",
  "delay": [
    {
      "id": "T82R9",
      "delayInSeconds": 12,
      "estimatedTime": "19:09",
      "headsign": "Strzyża PKM",
      "routeId": 9,
      "tripId": 82,
      "status": "REALTIME",
      "theoreticalTime": "19:09",
      "timestamp": "19:07:20",
      "trip": 3577762,
      "vehicleCode": 1022,
      "vehicleId": 416
    }, …
  ]
}
```

Rysunek 1 – JSON z estymowanymi czasami przyjazdów na przystanek Miszewskiego.

W ramach zadania laboratoryjnego proszę zaimplementować prostą aplikację pozwalającą na rejestrację i zalogowanie się użytkownika. Hasło zarejestrowanego użytkownika powinno być przechowywane w bazie danych w postaci hasha, np. z wykorzystaniem funkcji bcrypt. Logowanie odbywa się w oparciu o Bearer authentication. Po zalogowaniu wyświetla użytkownikowi jego spersonalizowane dane (w przykładowym kodzie jest to lista wybranych przez niego tablic przystankowych – osobna dla każdego użytkownika), która zawiera listę niebawem odjeżdżających z tego przystanku, pojazdów ZTM. Użytkownik powinien mieć możliwość dodawania, przeglądania, edycji i usuwania tych danych.

## Wymagania techniczne (backend):

- Encje modelu danych muszą wykorzystywać zamiast typów prostych obiekty wartościowe, zawierające logikę przynajmniej w postaci walidacji wartości.
- Dane użytkownika (login i hash hasła) przechowywane w bazie danych dokumentowej (np. MongoDB) albo relacyjnej (np. SQLite),
- Wykorzystanie odpowiedniego JavaScript ODM (np. Mongoose) albo ORM (np. EF Core) dla wybranej bazy danych.
- Autoryzacja z wykorzystaniem Bearer authentication.
- Zbudowanie API w node.js (np. z wykorzystaniem frameworka Express.js bądź Sails.js) albo w .NET (np. z wykorzystaniem ASP.NET Web API albo Minimal API) obsługującego utożsamianie użytkownika.
- Zapytania API dotyczące danych zalogowanego użytkownika powinny być chronione przez token JWT (Bearer authentication).
- API bazuje na danych z bazy danych oraz z publicznego API ZTM.
- Długotrwałe zapytanie z API ZTM, np. o listę wszystkich dostępnych przystanków zwracające ciężki plik stops.json powinno być cash’owane.
- Dokumentacja API z wykorzystaniem Swagger’a z możliwością zalogowania się użytkownika (zapamiętania tokenu JWT).
- (dodatkowe punkty) wizualizacja położenia autobusów/przystanków/tras z wykorzystaniem map, np.: Google/OSM itp…

## Wymagania techniczne (frontend w Vite + Vue.JS/React):

- Funkcjonalny podział GUI na komponenty jedno-plikowe.
- Implementacja przynajmniej jednego multikomponentu.
- Implementacja i wykorzystanie własnej dyrektywy.
- Implementacja przynajmniej jednej funkcji wielokrotnego użytku (Composable), np. do pobieranie danych z serwera.
- Nawigacja za pomocą dynamicznego komponentu (vue-router@4).
- Wykorzystać przynajmniej 2 dodatkowe biblioteki lub narzędzia (oprócz Pinia i Router), np.:
    - wtyczkę vue-good-table-next (omówiona w instrukcji ),
    - bibliotekę Axios LUB wbudowane w przeglądarkę Fetch API.
        - Ważne: W przypadku wyboru Fetch API, należy zaimplementować własny mechanizm (np. rozbudowując funkcję composable useFetch lub tworząc dedykowany serwis), który będzie automatycznie dołączał token JWT (pobrany z magazynu Pinia) do nagłówka Authorization dla wszystkich chronionych żądań do Twojego API.
- Zarządzanie stanem aplikacji z wykorzystaniem magazynu Pinia.
- Implementacja i użycie własnej wtyczki.
- Wykorzystanie frameworka Tailwind CSS.
- Kilka testów jednostkowych, w tym przynajmniej jednej funkcji wielokrotnego użytku (Composable).
- Kilka testów komponentów.
- Kilka testów E2E w wybranym frameworku (Selenium, Nightwatch … ).

## Podpowiedzi:

- **Backend:**
    - Do hashowania haseł w Express/Node.js najprościej użyć biblioteki bcryptjs (działa bez kompilacji natywnych modułów).
    - Problem cache'owania pliku stops.json można rozwiązać w prosty sposób: stwórzcie serwis, który pobiera plik raz, zapisuje go (np. w pamięci serwera lub jako plik tymczasowy) i ustawia datę wygaśnięcia (np. na 24 godziny). Każde kolejne żądanie do API będzie dostawało dane z pamięci podręcznej, dopóki nie wygaśnie.

- **Frontend:**
    - Użyjcie Pinia do przechowywania stanu zalogowania (tokena JWT i danych użytkownika).
    - Stwórzcie funkcję composable (na wzór useFetch ) lub instancję Axios, która automatycznie dołącza token JWT (pobrany z Pinia) do nagłówka Authorization dla wszystkich zapytań do waszego chronionego API.


## Punktacja:

### Backend (50 punktów)

- **Uwierzytelnianie i Użytkownicy (20 pkt):**
    - Poprawna rejestracja z hashowaniem haseł (bcrypt). (5 pkt)
    - Endpoint logowania zwracający poprawny token JWT. (5 pkt)
    - Middleware weryfikujący JWT i chroniący trasy. (5 pkt)
    - Konfiguracja Swaggera z obsługą autoryzacji Bearer. (5 pkt)
- **Logika Biznesowa i API (20 pkt):**
    - Pełne API CRUD do zarządzania zapisanymi przystankami użytkownika. (5 pkt)
    - Główny endpoint API, który pobiera zapisane przystanki z bazy oraz łączy je z danymi na żywo z ZTM. (10 pkt)
    - Implementacja cache'owania dla stops.json. (5 pkt)
- **Architektura i Baza Danych (10 pkt):**
    - Poprawna konfiguracja bazy danych (MongoDB/SQLite) z ODM/ORM (Mongoose/EF Core). (5 pkt)
    - Zastosowanie Obiektów Wartościowych (Value Objects) dla encji (np. dla loginu, hasha hasła). (5 pkt)

### Frontend (50 punktów)

- **Architektura i Funkcjonalność (20 pkt):**
    - Poprawna struktura komponentów (SFC, multikomponenty). (5 pkt)
    - Implementacja routingu (Vue Router) i nawigacji. (5 pkt)
    - Zarządzanie stanem (Pinia) do obsługi logowania i danych użytkownika. (5 pkt)
    - Komponenty poprawnie wyświetlające spersonalizowane dane (CRUD) oraz dane na żywo z ZTM. (5 pkt)
- **Zaawansowane Funkcje Vue (15 pkt):**
    - Implementacja własnej dyrektywy (np. do zmiany koloru opóźnienia). (5 pkt)
    - Implementacja własnej funkcji Composable (np. useZtmData). (5 pkt)
    - Implementacja własnej wtyczki (pluginu). (5 pkt)
- **Narzędzia i Testy (15 pkt):**
    - Użycie Tailwind CSS do stylizowania. (5 pkt)
    - Wykorzystanie 2 dodatkowych wtyczek (np. Axios + biblioteka do dat). (5 pkt)
    - Obecność testów (Unit, Component lub E2E) . (5 pkt)

## Punkty Dodatkowe (25 pkt):

- Wizualizacja na mapie (Google/OSM): +10 pkt




To świetny i ambitny projekt. Podejście "Clean Architecture" (Czysta Architektura) w .NET jest bardzo cenione w środowisku komercyjnym, więc wykonanie tego w ten sposób na studia zrobi duże wrażenie.

Oto szczegółowa analiza, rekomendacja dotycząca mikrofrontendów oraz struktura projektu.

-----

### 1\. Decyzja: Mikrofrontendy (Micro-frontends)

**Krótka odpowiedź:** **Pomiń mikrofrontendy.** Zrób to tradycyjnie (Monolityczny Frontend).

**Dlaczego?**
Mikrofrontendy rozwiązują problemy organizacyjne w wielkich zespołach (np. 50+ programistów), gdzie różne zespoły wdrażają różne części UI niezależnie.

* **Czas:** Wdrożenie mikrofrontendów (np. przez Module Federation w Vite) skomplikuje konfigurację projektu (webpack/vite config), zarządzanie stanem (współdzielenie Pinia między aplikacjami) i deployment. Dla jednej osoby to narzut czasowy rzędu **200-300%** w fazie konfiguracji.
* **Cel zadania:** Twoim celem jest pokazanie "Czystego Kodu" i logiki biznesowej, a nie walka z konfiguracją infrastruktury buildowania.
* [cite_start]**Zysk:** 15 punktów bonusowych [cite: 111] [cite_start]jest kuszące, ale szybciej zarobisz 10 punktów na mapie[cite: 109], która jest łatwiejsza i bardziej efektowna wizualnie.

Skupimy się więc na **Clean Architecture w Backendzie (.NET)** i **Modularnej Architekturze w Frontendzie (Vue 3)**.

-----

### 2\. Architektura Systemu (Clean Architecture)

W Clean Architecture najważniejsza jest **Zasada Zależności (Dependency Rule)**: warstwy wewnętrzne nie wiedzą nic o warstwach zewnętrznych.

#### Warstwy Backend (.NET)

1.  **Domain (Core)** - Serce systemu.

    * Nie ma żadnych zależności do innych projektów.
    * Zawiera **Encje** (np. `User`, `StopSubscription`).
    * [cite_start]Zawiera **Obiekty Wartościowe (Value Objects)** - to jest wymóg zadania[cite: 36, 91]. Np. `PasswordHash` (który sam sprawdza, czy hash jest poprawny) lub `StopId`.
    * Zawiera **Wyjątki domenowe**.

2.  **Application** - Logika działania aplikacji.

    * Zależy tylko od Domain.
    * Definiuje **Interfejsy** (np. `IUserRepository`, `IZtmService`, `IJwtProvider`).
    * Zawiera **Serwisy/Use Cases** (np. `LoginUserUseCase`, `GetDashboardDataService`).
    * Tu implementujesz logikę pobierania danych, łączenia ich z ZTM i zwracania do API.

3.  **Infrastructure** - Implementacja interfejsów (brudna robota).

    * Zależy od Application i Domain.
    * Baza danych: Konfiguracja **EF Core** (DbContext), Repozytoria.
    * ZTM Client: `HttpClient` do pobierania `stops.json` i `delays`.
    * [cite_start]**Caching**: Tu zaimplementujesz MemoryCache dla pliku `stops.json`[cite: 46, 71].
    * Auth: Implementacja generowania tokenów JWT.

4.  **API (Presentation)** - Punkt wejścia.

    * Zależy od Application.
    * Kontrolery lub Minimal API.
    * [cite_start]Konfiguracja Swaggera[cite: 47].
    * [cite_start]Middleware do obsługi błędów i weryfikacji JWT[cite: 83].

#### Warstwy Frontend (Vue 3 + Vite)

Zamiast płaskiej struktury, zastosuj strukturę opartą na **funkcjonalnościach (Features)**.

-----

### 3\. Struktura Katalogowa Projektu

Zakładam, że tworzysz jedno repozytorium (monorepo) z folderami `backend` i `frontend`.

#### BACKEND (Structure: .NET Solution)

Użyj polecenia `dotnet new sln` i dodaj 4 projekty (Class Library dla pierwszych trzech, Web API dla ostatniego).

```text
/backend
  /src
    /ZtmApp.Domain            (Class Library)
      /Entities               (np. User.cs, UserStop.cs)
      [cite_start]/ValueObjects           (np. Email.cs, PasswordHash.cs - wymagane [cite: 91])
      /Exceptions             (np. InvalidCredentialsException.cs)
      
    /ZtmApp.Application       (Class Library)
      /DTOs                   (np. UserDto.cs, LoginRequest.cs)
      /Interfaces             (np. IUserRepository.cs, IZtmService.cs)
      /Services               (np. AuthService.cs, DashboardService.cs)
      
    /ZtmApp.Infrastructure    (Class Library)
      /DAL                    (Data Access Layer)
        /Context              (AppDbContext.cs - EF Core)
        /Repositories         (UserRepository.cs)
        /Migrations
      /ExternalServices
        /Ztm                  (ZtmClient.cs - pobieranie z API ZTM)
      [cite_start]/Security               (JwtTokenGenerator.cs, PasswordHasher.cs [cite: 82])
      [cite_start]/Caching                (ZtmCacheService.cs - obsługa stops.json [cite: 71])
      
    /ZtmApp.Api               (ASP.NET Core Web API)
      /Controllers            (AuthController.cs, StopsController.cs)
      [cite_start]/Middleware             (ExceptionMiddleware.cs, JwtMiddleware.cs [cite: 83])
      /Program.cs             (Konfiguracja DI, Swaggera, Serwera)
      appsettings.json
```

#### FRONTEND (Structure: Vue 3 + Vite)

```text
/frontend
  /public
  /src
    /assets                   (Style, Tailwind input)
    
    /components               (Komponenty ogólnego użytku - "Dumb components")
      ui/                     (Button.vue, Input.vue, Modal.vue)
      layout/                 (Navbar.vue, Footer.vue)
      
    [cite_start]/composables              (Logika wielokrotnego użytku [cite: 53])
      useAuth.js              (Logowanie, rejestracja, trzymanie tokena)
      [cite_start]useZtmData.js           (Pobieranie danych ZTM - wymagane [cite: 100])
      [cite_start]useFetchWithAuth.js     (Wrapper na fetch/axios dodający nagłówek Bearer [cite: 59, 75])
      
    [cite_start]/directives               (Własne dyrektywy [cite: 52])
      [cite_start]v-delay-color.js        (Kolorowanie opóźnienia: zielony/czerwony [cite: 99])
      
    /features                 (Podział na moduły biznesowe)
      /auth
        /components           (LoginForm.vue, RegisterForm.vue)
      /dashboard
        [cite_start]/components           (StopList.vue, StopCard.vue - "Multi-komponenty" [cite: 51])
        /views                (DashboardView.vue)
      [cite_start]/map                    (Wizualizacja mapy [cite: 48])
      
    [cite_start]/plugins                  (Własne wtyczki [cite: 61])
      myLoggerPlugin.js
      
    [cite_start]/router                   (Vue Router [cite: 55])
      index.js
      
    [cite_start]/stores                   (Pinia [cite: 60])
      [cite_start]authStore.js            (Przechowywanie Usera i Tokena [cite: 74])
      
    App.vue
    main.js
  
  index.html
  vite.config.js
  [cite_start]tailwind.config.js          (Konfiguracja Tailwind [cite: 62])
```

-----

### 4\. Kluczowe elementy implementacji (Tip-y)

#### [cite_start]Backend: Cache'owanie `stops.json` [cite: 71, 88]

W warstwie `Infrastructure` stwórz serwis, który działa jako Singleton.

1.  Sprawdza, czy w pamięci (IMemoryCache) są dane.
2.  Jeśli tak -\> zwraca.
3.  Jeśli nie -\> pobiera z `https://ckan.multimediagdansk.pl/...`, parsuje, zapisuje w Cache z `AbsoluteExpiration` ustawionym np. na 24h i zwraca.

#### [cite_start]Backend: Value Objects (Obiekty Wartościowe) [cite: 36]

Nie używaj `string` dla hasła w encji. Zrób klasę:

```csharp
public class PasswordHash : ValueObject
{
    public string Value { get; }
    // Konstruktor, walidacja i logika porównywania
}
```

EF Core potrafi to mapować (tzw. Owned Entity lub Value Conversion). To jest wymagane w zadaniu i wysoko punktowane.

#### [cite_start]Frontend: `useFetchWithAuth` [cite: 59, 75]

Musisz napisać composable, który przechwytuje każde żądanie.

1.  Pobierz token z Pinia (`authStore`).
2.  Jeśli token istnieje, dodaj nagłówek `Authorization: Bearer {token}`.
3.  Wykonaj żądanie.

#### [cite_start]Frontend: Mapy [cite: 109]

Użyj biblioteki **Leaflet** (z wrapperem dla Vue, np. `@vue-leaflet/vue-leaflet`). Jest darmowa (OpenStreetMap) i o wiele prostsza w konfiguracji niż Google Maps API (które wymaga karty kredytowej). To da Ci łatwe +10 pkt.

### Następny krok

Czy chcesz, abym przygotował dla Ciebie **kod startowy** dla konkretnego elementu, np. definicję `ValueObject` w C\# dla Clean Architecture, albo przykład `composable` `useFetchWithAuth` w Vue?