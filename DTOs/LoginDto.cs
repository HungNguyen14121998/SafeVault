using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    public string username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }
}