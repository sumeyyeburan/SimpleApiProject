namespace SimpleApiProject.Models;
public class LoginRequest : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
}

