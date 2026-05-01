# SafeVaultApp

**SafeVaultApp** is an [ASP.NET Core](https://microsoft.com) project that demonstrates authentication and authorization workflows using **ASP.NET Core Identity** and [JWT (JSON Web Tokens)](https://jwt.io).

The application includes user registration, login, role assignment, and secure endpoint access.

## Features

*   **User Registration:** Supports assigning specific roles during sign-up.
*   **JWT Authentication:** Issues secure tokens upon successful login.
*   **Role-Based Authorization:** Restricts access to endpoints based on user roles.
*   **Event Logging:** Tracks registration and login activities.
*   **Configurable Database:** Easily adjust connection settings via `appsettings.json`.

## Build & Run

### Prerequisites

*   [.NET 9 SDK](https://microsoft.com)
*   SQL Server (Local or Remote)
*   [Git](https://git-scm.com)

### Steps to Run

1.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

2.  **Apply migrations:**
    Update the database schema using [Entity Framework Core](https://microsoft.com):
    ```bash
    dotnet ef database update
    ```

3.  **Run the application:**
    ```bash
    dotnet run
    ```
    The API will be available at: `https://localhost:5001` (or the port specified in `launchSettings.json`).

## Testing

You can test the API endpoints using [Postman](https://postman.com) or `curl`.

### 1. Register
**POST** `/api/account/register`

```json
{
  "username": "alice",
  "email": "alice@example.com",
  "password": "StrongPassword123!",
  "role": "Admin"
}
```

### 2. Login
**POST** `/api/account/login`

```json
{
  "username": "alice",
  "password": "StrongPassword123!"
}
```

**Response Example:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
```
*Include this token in the `Authorization: Bearer <token>` header to access protected resources.*

## Troubleshooting

**Error:** *Could not open a connection to SQL Server*

Check your connection string in the `appsettings.json` file:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SafeVaultDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**Verification Checklist:**
*   Ensure the SQL Server service is running.
*   Verify the instance name (e.g., `localhost` vs `localhost\SQLEXPRESS`).
*   Confirm the database has been created (run `dotnet ef database update`).
*   Adjust credentials if using SQL Authentication instead of Windows Authentication.

## Notes

*   **Default Branch:** `main`
*   **Logging:** Logs are written to the console by default; configure providers in `Program.cs`.
*   **Production Security:** Store your JWT secret keys in [Environment Variables](https://microsoft.com) or a Secret Manager.
*   **Attributes:** Use `[Authorize(Roles="Admin")]` or `[Authorize]` to protect your Controllers and Actions.
