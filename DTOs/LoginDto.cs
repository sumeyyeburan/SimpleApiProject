using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.DTOs;

public class LoginDto
{
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}
