using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Donation.Controllers
{
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SeedCities()
        {
            if (!_context.Cities.Any())
            {
                var cities = new List<City>
                {
                    // القاهرة الكبرى
                    new City { CityName = "القاهرة" },
                    new City { CityName = "الجيزة" },
                    new City { CityName = "القليوبية" },
                    new City { CityName = "6 أكتوبر" },
                    new City { CityName = "الشروق" },
                    new City { CityName = "العبور" },
                    
                    // الإسكندرية وشمال مصر
                    new City { CityName = "الإسكندرية" },
                    new City { CityName = "مطروح" },
                    new City { CityName = "البحيرة" },
                    
                    // الدلتا
                    new City { CityName = "المنصورة - الدقهلية" },
                    new City { CityName = "طنطا - الغربية" },
                    new City { CityName = "الزقازيق - الشرقية" },
                    new City { CityName = "شبين الكوم - المنوفية" },
                    new City { CityName = "دمياط" },
                    new City { CityName = "كفر الشيخ" },
                    new City { CityName = "بنها - القليوبية" },
                    
                    // القناة
                    new City { CityName = "بورسعيد" },
                    new City { CityName = "السويس" },
                    new City { CityName = "الإسماعيلية" },
                    new City { CityName = "شمال سيناء" },
                    new City { CityName = "جنوب سيناء" },
                    
                    // الصعيد الأوسط
                    new City { CityName = "الفيوم" },
                    new City { CityName = "بني سويف" },
                    new City { CityName = "المنيا" },
                    
                    // الصعيد الجنوبي
                    new City { CityName = "أسيوط" },
                    new City { CityName = "سوهاج" },
                    new City { CityName = "قنا" },
                    new City { CityName = "الأقصر" },
                    new City { CityName = "أسوان" },
                    
                    // البحر الأحمر
                    new City { CityName = "البحر الأحمر" },
                    new City { CityName = "الغردقة" },
                    
                    // الوادي الجديد
                    new City { CityName = "الوادي الجديد" }
                };

                _context.Cities.AddRange(cities);
                await _context.SaveChangesAsync();

                return Content("تم إضافة المحافظات المصرية بنجاح!");
            }

            return Content("المحافظات موجودة بالفعل!");
        }
    }
}
