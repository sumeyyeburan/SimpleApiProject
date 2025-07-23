using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleApiProject.Models
{
    public class UserClaim : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public Guid ClaimId { get; set; }

        [ForeignKey(nameof(ClaimId))]
        public virtual Claim Claim { get; set; }
    }
}
