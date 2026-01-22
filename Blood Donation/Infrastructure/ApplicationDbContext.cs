using Blood_Donation.Models;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<DonationMatch> DonationMatches { get; set; }
        public DbSet<DonationHistory> DonationHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Keys
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<City>()
                .HasKey(c => c.CityId);

            modelBuilder.Entity<Donor>()
                .HasKey(d => d.DonorId);

            modelBuilder.Entity<Patient>()
                .HasKey(p => p.PatientId);

            modelBuilder.Entity<BloodRequest>()
                .HasKey(br => br.RequestId);

            modelBuilder.Entity<DonationMatch>()
                .HasKey(dm => dm.MatchId);

            modelBuilder.Entity<DonationHistory>()
                .HasKey(dh => dh.DonationId);

            // Relations
            modelBuilder.Entity<User>()
                .HasOne(u => u.Donor)
                .WithOne(d => d.User)
                .HasForeignKey<Donor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .HasMany(c => c.Users)
                .WithOne(u => u.City)
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>()
                .HasMany(c => c.BloodRequests)
                .WithOne(br => br.City)
                .HasForeignKey(br => br.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.BloodRequests)
                .WithOne(br => br.Patient)
                .HasForeignKey(br => br.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donor>()
                .HasMany(d => d.DonationMatches)
                .WithOne(dm => dm.Donor)
                .HasForeignKey(dm => dm.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodRequest>()
                .HasMany(br => br.DonationMatches)
                .WithOne(dm => dm.BloodRequest)
                .HasForeignKey(dm => dm.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donor>()
                .HasMany(d => d.DonationHistories)
                .WithOne(dh => dh.Donor)
                .HasForeignKey(dh => dh.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BloodRequest>()
                .HasMany(br => br.DonationHistories)
                .WithOne(dh => dh.BloodRequest)
                .HasForeignKey(dh => dh.RequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
