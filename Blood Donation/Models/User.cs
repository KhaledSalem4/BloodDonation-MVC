namespace Blood_Donation.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; } // Donor / Patient / Admin

        public int CityId { get; set; }
        public City? City { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Donor? Donor { get; set; }
        public Patient? Patient { get; set; }
    }

}
