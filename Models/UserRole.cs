namespace SimpleApiProject.Models;
// Junction table for many-to-many relationship between User and Role
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}


