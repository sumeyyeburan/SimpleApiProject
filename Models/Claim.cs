using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models
{
    public class Claim
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } // e.g., "Permission"

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } // e.g., "CanDeleteUser"

        // Navigation property - Many users can have the same claim
        public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    }
}
