using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApiProject.Data;
using SimpleApiProject.Models;
using SimpleApiProject.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {
        private readonly QrService _qrService;
        private readonly AppDbContext _context;

        // Both QrService and AppDbContext should be injected
        public QrController(QrService qrService, AppDbContext context)
        {
            _qrService = qrService;
            _context = context;
        }

        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> GenerateQrCode([FromBody] QRCodeType type)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var (qrCode, qrCodeBase64) = await _qrService.CreateQrCode(userId, type);

            return Ok(new
            {
                qrCode.Id,
                qrCode.Type,
                qrCode.IsActive,
                qrCode.CreatedAt,
                qrCode.ExpiresAt,
                QrCodeBase64 = qrCodeBase64
            });
        }

        [HttpGet("validate/{id}")]
        public async Task<IActionResult> ValidateQrCode(Guid id)
        {
            var qrCode = await _context.QrCodes.FindAsync(id);

            if (qrCode == null)
                return NotFound("QR code not found.");

            if (qrCode.ExpiresAt < DateTime.UtcNow)
                return BadRequest("QR code has expired.");

            if (qrCode.Type == QRCodeType.one_time && qrCode.IsActive == false)
                return BadRequest("One-time QR code is invalid.");

            // If it's one-time: mark it as used
            if (qrCode.Type == QRCodeType.one_time)
            {
                qrCode.IsActive = false;
                qrCode.Status = BaseEntity.EntityStatus.Passive;
                qrCode.RevokedAt = DateTime.UtcNow;
                _context.QrCodes.Update(qrCode);
                await _context.SaveChangesAsync();
            }

            // If valid:
            return Ok(new
            {
                qrCode.Id,
                qrCode.Type,
                qrCode.IsActive,
                qrCode.ExpiresAt
            });
        }

    }
}
