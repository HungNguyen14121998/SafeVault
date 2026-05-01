using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using SafeValut.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Secret key (store securely, e.g., in appsettings.json or Azure Key Vault)
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();
await DataSeeder.SeedRolesAsync(app.Services);

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

// Serve the HTML form
app.MapGet("/", async context =>
{
    await context.Response.SendFileAsync("webform.html");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found."); ;

// Handle form submission
app.MapPost("/submit", async (HttpRequest request) =>
{
    var form = await request.ReadFormAsync();
    string username = form["username"];
    string email = form["email"];

    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();
        string sql = "INSERT INTO Users (Username, Email) VALUES (@Username, @Email)";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", email);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    return Results.Ok($"User {username} with email {email} added successfully!");
});

// Secure endpoint with JWT
app.MapGet("/secure", [Authorize] () =>
{
    return Results.Ok("This is protected data, only accessible with a valid JWT.");
});

// Minimal API example
app.MapGet("/admin", [Authorize(Roles = "Admin")] () =>
{
    return Results.Ok("Only Admins can access this.");
});

app.MapGet("/user", [Authorize(Roles = "User")] () =>
{
    return Results.Ok("Only Users can access this.");
});

app.MapGet("/guest", [Authorize(Roles = "Guest")] () =>
{
    return Results.Ok("Only Guests can access this.");
});

app.Run();
