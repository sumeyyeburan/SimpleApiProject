using SimpleApiProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

public enum QRCodeType
{
    permanent = 0,
    temporary = 1,
    one_time = 2
}

public class QrCode : BaseEntity
{
    public Guid UserId { get; set; }
    public QRCodeType Type { get; set; } = QRCodeType.permanent;
    public bool IsActive { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
