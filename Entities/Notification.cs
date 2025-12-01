namespace Korvan_API.Entities
{
    public enum NotificationType
    {
        ShiftAssigned,
        ShiftChanged,
        ShiftOffered,
        ShiftOfferAccepted
    }

    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public NotificationType Type { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid? RelatedShiftId { get; set; }
        public Guid? RelatedOfferId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
