namespace Korvan_API.Entities
{
	public class User
	{
		public Guid Id { get; set; } 
		public string Username { get; set; } = string.Empty;
		public string DisplayName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty; // e.g., "User", "Admin"
		public bool IsActive { get; set; } = true;

        public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }

		public string? ProfileColorHex { get; set; } // Profile color as hex
		public string? AvatarUrl { get; set; } // Profilepicture currently only URL based

		public ICollection<WorkShift> AssignedShifts { get; set; } = new List<WorkShift>();
    }
}
