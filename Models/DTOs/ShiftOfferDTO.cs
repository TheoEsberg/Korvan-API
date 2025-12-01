using Korvan_API.Entities;

namespace Korvan_API.Models.DTOs
{
    public class ShiftOfferDTO
    {
        public Guid Id { get; set; }
        public Guid ShiftId { get; set; }

        public Guid OfferedById { get; set; }
        public string OfferedByName { get; set; } = string.Empty;

        public Guid? TakenById { get; set; }
        public string? TakenByName { get; set; }

        public ShiftOfferType Type { get; set; }
        public ShiftOfferStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
