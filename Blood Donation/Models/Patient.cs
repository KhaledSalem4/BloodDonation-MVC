namespace Blood_Donation.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        // Navigation
        public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    }
}
