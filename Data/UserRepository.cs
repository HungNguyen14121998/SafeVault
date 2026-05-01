using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SafeValut.Areas.Identity.Data;

public class UserRepository
{
    private readonly ApplicationDbContext _context;

    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // old way
    public bool LoginUser(string username, string password)
    {
        string allowedSpecialCharacters = "!@#$%^&*?";

        // Validate inputs before hitting the database
        if (!ValidationHelpers.IsValidInput(username) ||
            !ValidationHelpers.IsValidInput(password, allowedSpecialCharacters))
        {
            return false;
        }

        const string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";

        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            // Always use parameters to avoid SQL injection
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", password);
            // ⚠️ In production: hash the password before storing/validating!

            connection.Open();
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
    }

    public bool AuthenticateUser(string username, string password)
    {
        const string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Username", username);

        conn.Open();
        var result = cmd.ExecuteScalar();
        if (result == null) return false;

        string storedHash = (string)result;
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

    // old way
    public bool RegisterUser(string username, string email, string password)
    {
        // Hash the password securely
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        const string query = "INSERT INTO Users (Username, Email, PasswordHash, Role) VALUES (@Username, @Email, @PasswordHash, @Role)";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@Role", "user"); // default role

        conn.Open();
        int rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public void TestXssInput()
    {
        string maliciousInput = "<script>alert('XSS');</script>";
        bool isValid = ValidationHelpers.IsValidXSSInput(maliciousInput);
        Console.WriteLine(isValid ? "XSS Test Failed" : "XSS Test Passed");
    }
}