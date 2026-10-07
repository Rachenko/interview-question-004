# interview-question-004

Profile registration app (IT 04-1) — built as if developed at **example.com**.

- **Backend:** ASP.NET Core (.NET 10), Clean Architecture (`Example.Domain` / `Example.Application` / `Example.Infrastructure` / `Example.Api`), EF Core
- **Frontend:** Angular 22
- **Database:** PostgreSQL 16 via `docker-compose.yml`

## Features

- Registration form (IT 04-1): First Name, Last Name, Email, Phone, Profile (image stored as Base64), Birth Day (`dd/MM/yyyy`), Occupation (combo box, master data from `occupations` table), Sex (Male/Female radio)
- Validation before saving — every field is required; Email, Phone and Birth Day are format-checked on both the form and the API
- `Save` posts to `POST /api/persons`, shows `save data success Id : {id}` and clears the form
- `Clear` resets the form

## Project structure

```
backend/
  src/Example.Domain/           Entities (Person, Occupation, Sex)
  src/Example.Application/      RegisterPersonRequest, validator, IPersonService/PersonService, IOccupationService, repositories
  src/Example.Infrastructure/   AppDbContext, PersonRepository, OccupationRepository, EF migrations (occupations seeded via SQL insert)
  src/Example.Api/              PersonsController, OccupationsController, DI, CORS, auto-migrate on start
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
# OpenAPI spec (dev): http://localhost:5134/openapi/v1.json

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
    "occupationId": 1,
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

## Troubleshooting

### npm : File C:\Program Files\nodejs\npm.ps1 cannot be loaded because running scripts is disabled on this system

PowerShell บล็อก `npm.ps1` เพราะ execution policy — แก้โดยรันคำสั่งนี้ใน PowerShell ครั้งเดียว:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

แล้วรัน `npm install` อีกครั้ง

ถ้าไม่อยากเปลี่ยน policy ให้ใช้ `npm.cmd install` หรือรันใน Command Prompt (cmd) แทน PowerShell

### ERR_CONNECTION_REFUSED ที่ http://localhost:5134/api/persons

Backend ยังไม่ได้รัน — เปิด terminal ใหม่แล้ว:

```bash
cd backend/src/Example.Api
dotnet run
```

รอจนขึ้น `Now listening on: http://localhost:5134` แล้วค่อยเปิดหน้า frontend หรือเรียก API

### error MSB3021/MSB3027: The file is locked by "Example.Api.exe"

เกิดจากตัว API ตัวเก่ายังรันค้างอยู่ล็อก DLL ใน `bin\Debug` — ไม่ใช่บั๊กโค้ด

แก้โดยปิด process เก่าก่อน build/run ใหม่:

```powershell
# หา PID ที่ล็อก (เลขในวงเล็บท้าย error เช่น 25524)
taskkill /PID <เลข PID> /F

# หรือฆ่าทุกตัวที่เกี่ยวข้อง
taskkill /F /IM Example.Api.exe
taskkill /F /IM dotnet.exe
```

ถ้า `taskkill` ขึ้น `Access is denied` หรือ process มี `Handles = 0` (zombie) ให้เปิด PowerShell แบบ **Run as administrator** แล้ว `Stop-Process -Name Example.Api -Force` — หรือ **Restart เครื่อง** ทีเดียวจบ

**ทางลัดไม่ต้อง kill:** build ไปอีก configuration เพื่อเลี่ยงโฟลเดอร์ที่ถูกล็อก

```powershell
dotnet run --project backend/src/Example.Api -c Release
```

**กันไม่ให้เกิดซ้ำ:**
- รัน API แค่ terminal เดียว — อย่าเปิด `dotnet run` ซ้อนกัน หรือซ้อนกับ `dotnet watch run` / debug ใน VS Code
- จะ build/test ให้ Ctrl+C ปิดตัวที่รันก่อน หรือเปิด terminal ใหม่สำหรับ `dotnet test`
- ใช้ `dotnet watch run --project backend/src/Example.Api` ถ้าอยากให้ rebuild+restart เองตอนแก้โค้ด

### อยากได้ Swagger UI

Repo นี้ใช้ OpenAPI built-in ของ .NET (spec อยู่ที่ `/openapi/v1.json` ตอน dev) ไม่ได้ติดตั้ง Swashbuckle — ถ้าอยากได้หน้า UI ให้ติดตั้ง `Swashbuckle.AspNetCore` เวอร์ชัน **10.x** ขึ้นไป (v9 ไม่เข้ากับ .NET 10 / Microsoft.OpenApi 2.x จะเจอ TypeLoadException)
