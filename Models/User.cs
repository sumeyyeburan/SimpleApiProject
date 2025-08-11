using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models;

public class User : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    // QR Kodlarla ilişki (1 kullanıcı, n QR kod)
    public ICollection<QrCode> QrCodes { get; set; } = new List<QrCode>();
}