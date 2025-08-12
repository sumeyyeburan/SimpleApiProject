using QRCoder;
using SimpleApiProject.Data;
using SimpleApiProject.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimpleApiProject.Services
{
    public class QrService
    {
        private readonly AppDbContext _context;

        public QrService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a QR code from the given data and returns it as a Base64 encoded PNG image.
        /// </summary>
        private string GenerateQrCodeBase64(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);

            using Bitmap qrBitmap = qrCode.GetGraphic(20);
            using var ms = new MemoryStream();
            qrBitmap.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/png;base64,{base64}";
        }

        /// <summary>
        /// Creates a QR code entry in the database and returns the entity along with its Base64 image representation.
        /// </summary>
        /// <param name="userId">The ID of the user creating the QR code.</param>
        /// <param name="type">The type of the QR code (e.g., one-time, reusable).</param>
        public async Task<(QrCode qrCode, string base64Image)> CreateQrCode(string userId, QRCodeType type)
        {
            var now = DateTime.UtcNow;
            var expireAt = now.AddMinutes(60);  // QR code expires in 60 minutes

            var qrCode = new QrCode
            {
                UserId = Guid.Parse(userId),
                Type = type,
                CreatedBy = Guid.Parse(userId),
                UpdatedBy = Guid.Parse(userId),
                Status = BaseEntity.EntityStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
                ExpiresAt = expireAt,
                IsActive = true
            };

            // If the QR code type is one-time, mark it as inactive immediately
            if (type == QRCodeType.one_time)
            {
                qrCode.IsActive = false;
                qrCode.Status = BaseEntity.EntityStatus.Passive;
                qrCode.UpdatedAt = DateTime.UtcNow;
            }

            _context.QrCodes.Add(qrCode);
            await _context.SaveChangesAsync();

            // Generate the Base64 image from the QR code's unique ID
            string qrCodeData = qrCode.Id.ToString();
            string base64Image = GenerateQrCodeBase64(qrCodeData);

            return (qrCode, base64Image);
        }
    }

}
