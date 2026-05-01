using System;

public class User
{
    public int Id { get; set; }                // Primary key
    public string UserName { get; set; }       // Login name
    public string Email { get; set; }          // Email address
    public string PasswordHash { get; set; }   // Hashed password
    public string Role { get; set; }           // e.g., "Admin", "User"
    public DateTime CreatedAt { get; set; }    // When account was created
}
