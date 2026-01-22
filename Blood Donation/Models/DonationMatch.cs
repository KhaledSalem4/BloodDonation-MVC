namespace Blood_Donation.Models
{
    public class DonationMatch
    {
        public int MatchId { get; set; }

        public int RequestId { get; set; }
        public BloodRequest? BloodRequest { get; set; }

        public int DonorId { get; set; }
        public Donor? Donor { get; set; }

        public required string MatchStatus { get; set; } // Pending / Accepted / Rejected
        public DateTime MatchedAt { get; set; }
    }

}
