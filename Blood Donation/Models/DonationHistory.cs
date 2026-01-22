namespace Blood_Donation.Models
{
    public class DonationHistory
    {
        public int DonationId { get; set; }

        public int DonorId { get; set; }
        public Donor? Donor { get; set; }

        public int RequestId { get; set; }
        public BloodRequest? BloodRequest { get; set; }

        public DateTime DonationDate { get; set; }
    }

}
