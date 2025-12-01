namespace Korvan_API.Entities
{
    public enum ShiftOfferType
    {
        Giveaway,   // Employee offers their shift for free
        Swap,       // Employee wants to swap their shift with another
    }

    public enum ShiftOfferStatus
    {
        Pending,    // Offer created but not yet acted upon
        Accepted,   // Offer accepted by another employee
        Declined,   // Offer declined by another employee
        Cancelled   // Offer cancelled by the original employee
    }

    public class ShiftOffer
    {
        public Guid Id { get; set; }

        public Guid ShiftId { get; set; }
        public WorkShift Shift { get; set; } = null!;

        public Guid OfferedById { get; set; }
        public User OfferedBy { get; set; } = null!;

        public Guid? TakenById { get; set; }
        public User? TakenBy { get; set; }

        public ShiftOfferType Type { get; set; } = ShiftOfferType.Giveaway;
        public ShiftOfferStatus Status { get; set; } = ShiftOfferStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }
}
