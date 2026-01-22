namespace Blood_Donation.Models
{
    public class City
    {
        public int CityId { get; set; }
        public required string CityName { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    }

}
