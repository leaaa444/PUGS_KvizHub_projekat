# KvizHub - Platforma za testiranje znanja

## O Projektu

KvizHub je moderna veb aplikacija napravljena kao platforma za testiranje znanja. Korisnicima omogućava da rešavaju kvizove u dva režima: klasičnom **Solo režimu** i uzbudljivoj **Live Areni**, gde se takmiče protiv drugih igrača u realnom vremenu. Aplikacija poseduje bogat administratorski panel za upravljanje sadržajem, detaljne preglede rezultata i globalne rang liste.

Projekat je razvijen u skladu sa savremenim standardima frontend i backend razvoja, sa fokusom na modularnost, proširivost i čistu arhitekturu.

### Ključne Funkcionalnosti

* **Dva Režima Igre:**
    * [cite_start]**Solo Kviz:** Klasično rešavanje kvizova sa vremenskim ograničenjem[cite: 8].
    * [cite_start]**Live Quiz Arena:** Takmičenje više korisnika istovremeno u realnom vremenu, sa sinhronizovanim pitanjima i rang listom koja se ažurira uživo[cite: 112, 113].
* [cite_start]**Upravljanje Korisnicima:** Registracija i prijava sa JWT autentifikacijom[cite: 6, 33]. [cite_start]Lozinke se bezbedno čuvaju heširanjem[cite: 30, 100].
* [cite_start]**Baza Kvizova:** Pregled, pretraga i filtriranje kvizova po nazivu, kategoriji i težini[cite: 7, 39].
* [cite_start]**Raznovrsna Pitanja:** Podrška za četiri tipa pitanja: jedan tačan odgovor, više tačnih odgovora, tačno/netačno i unos teksta[cite: 19].
* [cite_start]**Analiza Rezultata:** Detaljan pregled rezultata nakon svakog kviza [cite: 9][cite_start], istorija ličnih rezultata [cite: 10] [cite_start]i globalna rang lista[cite: 11, 73].
* [cite_start]**Administratorski Panel:** Kompletan CRUD (Create, Read, Update, Delete) interfejs za upravljanje kvizovima, pitanjima i kategorijama[cite: 13, 15].

## Tehnologije

### Backend
* **ASP.NET Core Web API** (.NET 8)
* [cite_start]**Entity Framework Core 8** (Code-First pristup sa SQL migracijama [cite: 88])
* [cite_start]**SignalR** (za komunikaciju u realnom vremenu u Live Areni [cite: 115])
* **SQL Server** (ili druga relacion baza podataka)
* [cite_start]**JWT (JSON Web Tokens)** za autentifikaciju i autorizaciju [cite: 33, 101]
* [cite_start]**AutoMapper** (za mapiranje između modela baze i DTO objekata [cite: 97])
* [cite_start]**Troslojna arhitektura** (Core, Infrastructure, API) [cite: 96]

### Frontend
* **React** (sa TypeScript-om)
* **React Router** (za navigaciju)
* [cite_start]**Axios** (za HTTP zahteve ka backendu, organizovano kroz servise [cite: 93])
* **SignalR Client** (za povezivanje na QuizHub)
* [cite_start]**CSS Modules / Styled Components** (za stilizovanje komponenti [cite: 91])
* **Vite** (kao alat za build)

## Uputstvo za Pokretanje

### Preduslovi
* .NET 8 SDK
* Node.js (v18+)
* SQL Server (ili drugi DB po izboru)
* Git

### 1. Backend Pokretanje

```bash
# 1. Klonirajte repozitorijum
git clone [URL_VAŠEG_REPOZITORIJUMA]
cd [NAZIV_REPOZITORIJUMA]/backend

# 2. Konfiguracija baze podataka
# Otvorite `appsettings.json` i podesite `ConnectionString` za vašu bazu podataka.
# Primer:
# "ConnectionStrings": {
#   "DefaultConnection": "Server=localhost;Database=KvizHubDB;Trusted_Connection=True;TrustServerCertificate=True;"
# }

# 3. Primena migracija
# Otvorite Package Manager Console u Visual Studio (ili koristite .NET CLI)
dotnet ef database update

# 4. Pokretanje servera
dotnet run
```
[cite_start]Backend server će biti pokrenut, najverovatnije na `https://localhost:7142`. [cite: 92]

### 2. Frontend Pokretanje

```bash
# 1. Pozicionirajte se u frontend folder
cd ../frontend

# 2. Instalirajte zavisnosti
npm install

# 3. Podesite promenljive okruženja
# Kreirajte `.env` fajl u `frontend` folderu i dodajte URL vašeg backend servera.
# Primer `.env` fajla:
REACT_APP_API_BASE_URL=https://localhost:7142

# 4. Pokrenite klijentsku aplikaciju
npm start
```
Frontend aplikacija će biti dostupna na `http://localhost:3000`.

## Arhitektura Sistema

Aplikacija je dizajnirana po klijent-server arhitekturi.

* [cite_start]**Zadnja strana (Backend)** je izgrađena kao **troslojna Web API aplikacija**[cite: 96], prateći principe Dependency Injection-a. [cite_start]Jasno su razdvojeni modeli baze podataka od DTO (Data Transfer Object) modela koji se koriste za komunikaciju sa klijentom[cite: 97]. [cite_start]REST konvencija je ispoštovana za nazivanje resursa[cite: 98].
* **Prednja strana (Frontend)** je **Single Page Application (SPA)** izgrađena u React-u. [cite_start]Aplikacija je podeljena po komponentama [cite: 91][cite_start], a komunikacija sa backendom je enkapsulirana unutar posebnih servisa[cite: 93]. [cite_start]Modeli na prednjoj strani (TypeScript interfejsi) osiguravaju tipsku sigurnost podataka[cite: 95].

[cite_start]Komunikacija za standardne operacije se obavlja preko RESTful API-ja, dok se za funkcionalnosti Live Arene koristi **SignalR** za dvosmernu komunikaciju u realnom vremenu[cite: 115].
