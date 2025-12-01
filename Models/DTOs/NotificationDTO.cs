using Korvan_API.Entities;

namespace Korvan_API.Models.DTOs
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid? RelatedShiftId { get; set; }
        public Guid? RelatedOfferId { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

    }
}
