# SafeVaultApp

SafeVaultApp is an ASP.NET Core project that demonstrates **authentication and authorization** using **ASP.NET Core Identity** and **JWT (JSON Web Tokens)**.  
It includes user registration, login, role assignment, and secure endpoint access.

---

## 🚀 Features

- User registration with roles
- Login with JWT token issuance
- Role-based authorization
- Logging of login and registration events
- Configurable database connection via `appsettings.json`

---

## 🛠️ Build & Run

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)
- Git

### Steps

**Restore dependencies:**

```bash
dotnet restore
Apply migrations and create the database:

bash
dotnet ef database update
Run the application:

bash
dotnet run
The API will be available at https://localhost:5001 (or the port configured in launchSettings.json).

🧪 Testing
You can test endpoints using Postman or curl.

Register
bash
POST https://localhost:5001/api/account/register
Content-Type: application/json

{
  "username": "alice",
  "email": "alice@example.com",
  "password": "StrongPassword123!",
  "role": "Admin"
}
Login
bash
POST https://localhost:5001/api/account/login
Content-Type: application/json

{
  "username": "alice",
  "password": "StrongPassword123!"
}
Response:

json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
Use this token in the Authorization: Bearer <token> header to access protected endpoints.

⚠️ Troubleshooting
Error: Could not open a connection to SQL Server
If you see errors like:

Code
A network-related or instance-specific error occurred while establishing a connection to SQL Server.
Check your database configuration in appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SafeVaultDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Ensure SQL Server is running.

Verify the instance name (localhost vs localhost\\SQLEXPRESS).

Confirm the database exists (run dotnet ef database update).

Adjust credentials if using SQL Authentication.

📌 Notes`
Default branch is main.

Logs are written to console by default; configure logging providers in Program.cs as needed.

For production, secure your JWT key in environment variables or a secrets manager.`

Use [Authorize(Roles="Admin")] or [Authorize] attributes to protect endpoints.
```
