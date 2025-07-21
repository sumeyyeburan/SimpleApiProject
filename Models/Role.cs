using System.ComponentModel.DataAnnotations;

namespace SimpleApiProject.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // e.g., "Admin", "User"

        // Navigation property - Many users can have this role
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
