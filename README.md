# interview-question-004

Profile registration app (IT 04-1) — built as if developed at **example.com**.

- **Backend:** ASP.NET Core (.NET 10), Clean Architecture (`Example.Domain` / `Example.Application` / `Example.Infrastructure` / `Example.Api`), EF Core
- **Frontend:** Angular 22
- **Database:** PostgreSQL 16 via `docker-compose.yml`

## Features

- Registration form (IT 04-1): First Name, Last Name, Email, Phone, Profile (image stored as Base64), Birth Day (`dd/MM/yyyy`), Occupation (combo box, mock data), Sex (Male/Female radio)
- Validation before saving — every field is required; Email, Phone and Birth Day are format-checked on both the form and the API
- `Save` posts to `POST /api/persons`, shows `save data success Id : {id}` and clears the form
- `Clear` resets the form

## Project structure

```
backend/
  src/Example.Domain/           Entities (Person, Sex)
  src/Example.Application/      RegisterPersonRequest, validator, handler, IPersonRepository
  src/Example.Infrastructure/   AppDbContext, PersonRepository, EF migrations
  src/Example.Api/              PersonsController, DI, CORS, auto-migrate on start
  tests/Example.Application.Tests/  xUnit tests (validator + handler)
frontend/
  src/app/person-form/          Registration form component + PersonService
docker-compose.yml              PostgreSQL 16
```

## Prerequisites

- .NET SDK 10
- Node.js 22+ (developed on Node 24)
- Docker (for PostgreSQL)

## Run

```bash
# 1. Start PostgreSQL
docker compose up -d

# 2. Start the API (http://localhost:5134, applies migrations on startup)
cd backend
dotnet run --project src/Example.Api

# 3. Start the frontend (http://localhost:4200)
cd frontend
npm install
npm start
```

Open http://localhost:4200 and submit the form. The API connection string is in
`backend/src/Example.Api/appsettings.json` (`Host=localhost;Port=5432;Database=profile_registration;User=postgres;Password=postgres`).

### API example

```bash
curl -X POST http://localhost:5134/api/persons \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Somchai",
    "lastName": "Jaidee",
    "email": "somchai@example.com",
    "phone": "081-234-5678",
    "profile": "AQID",
    "birthDay": "15/08/1995",
    "occupation": "Software Developer",
    "sex": "male"
  }'
# -> {"id": 1}
```

Validation failures return `400` with a map of field → message, e.g.
`{"errors": {"email": "Please provide a valid Email"}}`.

## Tests

```bash
# Backend (xUnit: validator rules + handler/save flow)
cd backend
dotnet test

# Frontend (vitest: component + birthDay validator)
cd frontend
npm test
```

## Reset the database

```bash
docker compose down -v && docker compose up -d
```
