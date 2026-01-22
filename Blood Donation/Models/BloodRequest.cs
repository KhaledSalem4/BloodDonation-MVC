namespace Blood_Donation.Models
{
    public class BloodRequest
    {
        public int RequestId { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public required string BloodTypeNeeded { get; set; }
        public required string HospitalName { get; set; }
        public int CityId { get; set; }
        public City? City { get; set; }

        public required string UrgencyLevel { get; set; } // Low / Medium / High
        public string? Notes { get; set; }
        public required string Status { get; set; } // Open / InProgress / Closed
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<DonationMatch> DonationMatches { get; set; } = new List<DonationMatch>();
        public ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();
    }

}
