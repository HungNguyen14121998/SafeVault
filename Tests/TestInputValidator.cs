// Tests/TestInputValidation.cs
using NUnit.Framework;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        // Example of SQL injection attempt
        string maliciousInput = "42 OR 1=1";

        bool isSafe = ValidationHelpers.IsValidInput(maliciousInput);

        // Assert.IsFalse(isSafe, "SQL injection attempt should be detected as unsafe.");
    }

    [Test]
    public void TestForXSS()
    {
        // Example of XSS attempt
        string maliciousInput = "<script>alert('xss')</script>";

        bool isSafe = ValidationHelpers.IsValidInput(maliciousInput);

        // Assert.IsFalse(isSafe, "XSS attempt should be detected as unsafe.");
    }

    [Test]
    public void TestForValidInput()
    {
        // Example of safe input
        string safeInput = "User123";

        bool isSafe = ValidationHelpers.IsValidInput(safeInput);

        // Assert.IsTrue(isSafe, "Normal alphanumeric input should be considered safe.");
    }
}