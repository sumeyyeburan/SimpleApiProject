namespace SimpleApiProject.Models;
// Junction table for many-to-many relationship between User and Claim
public class UserClaim : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid ClaimId { get; set; }
    public Claim Claim { get; set; }
}

