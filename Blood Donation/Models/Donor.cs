namespace Blood_Donation.Models
{
    public class Donor
    {
        public int DonorId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public required string BloodType { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public bool IsAvailable { get; set; }
        public required string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Navigation
        public ICollection<DonationMatch> DonationMatches { get; set; } = new List<DonationMatch>();
        public ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();
    }

}
