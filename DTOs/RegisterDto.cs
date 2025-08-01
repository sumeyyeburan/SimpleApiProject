using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*_*]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters, include an uppercase letter, a number, and a special symbol.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; }
}


