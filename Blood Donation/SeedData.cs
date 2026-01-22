using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using System.Security.Cryptography;
using System.Text;

namespace Blood_Donation
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Cities if not exist
            if (!context.Cities.Any())
            {
                var cities = new List<City>
                {
                    // محافظات القاهرة الكبرى
                    new City { CityName = "القاهرة" },
                    new City { CityName = "الجيزة" },
                    new City { CityName = "القليوبية" },
                    new City { CityName = "6 أكتوبر" },
                    new City { CityName = "الشيخ زايد" },
                    new City { CityName = "مدينة نصر" },
                    new City { CityName = "مصر الجديدة" },
                    new City { CityName = "المعادي" },

                    // محافظات الدلتا
                    new City { CityName = "الإسكندرية" },
                    new City { CityName = "الدقهلية" },
                    new City { CityName = "الشرقية" },
                    new City { CityName = "الغربية" },
                    new City { CityName = "كفر الشيخ" },
                    new City { CityName = "المنوفية" },
                    new City { CityName = "البحيرة" },
                    new City { CityName = "دمياط" },

                    // محافظات القناة
                    new City { CityName = "بورسعيد" },
                    new City { CityName = "الإسماعيلية" },
                    new City { CityName = "السويس" },

                    // محافظات الصعيد
                    new City { CityName = "الفيوم" },
                    new City { CityName = "بني سويف" },
                    new City { CityName = "المنيا" },
                    new City { CityName = "أسيوط" },
                    new City { CityName = "سوهاج" },
                    new City { CityName = "قنا" },
                    new City { CityName = "الأقصر" },
                    new City { CityName = "أسوان" },

                    // محافظات البحر الأحمر وسيناء
                    new City { CityName = "البحر الأحمر" },
                    new City { CityName = "جنوب سيناء" },
                    new City { CityName = "شمال سيناء" },
                    new City { CityName = "مطروح" },
                    new City { CityName = "الوادي الجديد" }
                };

                await context.Cities.AddRangeAsync(cities);
                await context.SaveChangesAsync();
            }

            // Seed Admin User if not exist
            if (!context.Users.Any(u => u.Email == "admin@blooddonation.com"))
            {
                var defaultCity = context.Cities.First();

                var adminUser = new User
                {
                    FullName = "مدير النظام",
                    Email = "admin@blooddonation.com",
                    PhoneNumber = "01000000000",
                    PasswordHash = HashPassword("Admin@123"),
                    Role = "Admin",
                    CityId = defaultCity.CityId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ Admin user created successfully!");
                Console.WriteLine("📧 Email: admin@blooddonation.com");
                Console.WriteLine("🔑 Password: Admin@123");
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
