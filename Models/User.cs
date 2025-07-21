using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

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

        // Date the user was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    }
}
