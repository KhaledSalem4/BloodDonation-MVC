# 🩸 نَبْضَة - Blood Donation Management System

<div align="center">

![Blood Donation](https://img.shields.io/badge/Blood-Donation-red?style=for-the-badge)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-purple?style=for-the-badge)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-blue?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**منصة إلكترونية متكاملة لإدارة التبرع بالدم وربط المتبرعين بالمرضى المحتاجين**

[المميزات](#-المميزات) • [التقنيات](#-التقنيات-المستخدمة) • [التثبيت](#-التثبيت-والإعداد) • [الاستخدام](#-طريقة-الاستخدام)

</div>

---

## 📋 نظرة عامة

**نَبْضَة** هو نظام متقدم لإدارة عمليات التبرع بالدم، يوفر منصة سهلة وآمنة لربط المتبرعين بالدم مع المرضى المحتاجين إليه. يتميز النظام بواجهة مستخدم عصرية وجذابة مع دعم كامل للغة العربية والتصميم من اليمين لليسار (RTL).

### 🎯 الهدف

تسهيل عملية التبرع بالدم وإنقاذ الأرواح من خلال:
- ربط المتبرعين بالمرضى بسرعة وفعالية
- نظام مطابقة ذكي لفصائل الدم
- إدارة شاملة لطلبات التبرع
- متابعة تاريخ التبرعات

---

## ✨ المميزات

### 🔐 نظام المصادقة والأمان
- تسجيل دخول آمن باستخدام Cookie Authentication
- تشفير كلمات المرور بـ SHA256
- أدوار متعددة (متبرع، مريض، مسؤول)
- صفحات محمية حسب الصلاحيات

### 👥 إدارة المستخدمين
- **للمتبرعين:**
  - إنشاء وتحديث الملف الشخصي
  - عرض طلبات التبرع المتاحة
  - تاريخ التبرعات السابقة
  - طلب التبرع للحالات المتاحة

- **للمرضى:**
  - إنشاء طلبات تبرع بالدم
  - إدارة الطلبات (فتح/إغلاق)
  - عرض قائمة المتبرعين المنتظرين
  - قبول أو رفض المتبرعين
  - الحصول على معلومات التواصل

### 🔄 نظام المطابقة الذكي
- مطابقة تلقائية لفصائل الدم المتوافقة
- فلترة حسب الموقع الجغرافي
- نظام حالات متقدم (معلق، مقبول، مرفوض)
- إشعارات للمتبرعين والمرضى

### 📊 لوحة تحكم المسؤول
- إحصائيات شاملة للنظام
- إدارة جميع المستخدمين
- مراقبة طلبات التبرع
- إدارة عمليات المطابقة
- تفعيل/تعطيل الحسابات

### 🎨 واجهة مستخدم متقدمة
- تصميم عصري وجذاب
- دعم كامل للغة العربية (RTL)
- شعار متحرك "نَبْضَة"
- Navbar ديناميكي حسب حالة المستخدم
- رسوم متحركة و Gradients
- Responsive Design لجميع الأجهزة

---

## 🛠 التقنيات المستخدمة

### Backend
- **ASP.NET Core 9.0** - Framework رئيسي
- **Entity Framework Core** - ORM لإدارة قاعدة البيانات
- **SQL Server** - قاعدة البيانات
- **Cookie Authentication** - نظام المصادقة
- **MVC Pattern** - نمط معماري

### Frontend
- **Bootstrap 5** - UI Framework
- **Font Awesome 6** - الأيقونات
- **CSS3 with Gradients & Animations** - التصميم
- **JavaScript & jQuery** - التفاعل
- **Google Fonts (Cairo)** - الخطوط العربية

### قاعدة البيانات
```
Models:
├── User (المستخدمون)
├── Donor (المتبرعون)
├── Patient (المرضى)
├── BloodRequest (طلبات الدم)
├── DonationMatch (المطابقات)
├── DonationHistory (تاريخ التبرعات)
└── City (المدن)
```

---

## 🚀 التثبيت والإعداد

### المتطلبات الأساسية
```bash
- .NET SDK 9.0 أو أحدث
- SQL Server 2019 أو أحدث
- Visual Studio 2022 أو VS Code
- Git
```

### خطوات التثبيت

1. **استنساخ المشروع**
```bash
git clone https://github.com/KhaledSalem4/BloodDonation-MVC.git
cd BloodDonation-MVC
```

2. **تحديث Connection String**
افتح `appsettings.json` وحدث connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BloodDonationDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. **تطبيق Migrations**
```bash
cd "Blood Donation"
dotnet ef database update
```

4. **تشغيل المشروع**
```bash
dotnet run
```
أو من Visual Studio: اضغط F5

5. **الوصول للتطبيق**
```
https://localhost:5001
```

---

## 📖 طريقة الاستخدام

### حسابات تجريبية (بعد تشغيل SeedData)

#### 👨‍⚕️ Admin
```
Email: admin@blooddonation.com
Password: Admin@123
```

#### 🩸 متبرع
```
Email: donor1@test.com
Password: Donor@123
```

#### 🏥 مريض
```
Email: patient1@test.com
Password: Patient@123
```

### سير العمل الأساسي

1. **للمتبرع:**
   - تسجيل دخول → عرض طلبات التبرع → الضغط على "أريد التبرع" → انتظار قبول المريض

2. **للمريض:**
   - تسجيل دخول → إنشاء طلب دم جديد → انتظار المتبرعين → قبول/رفض المتبرعين → التواصل مع المتبرع المقبول

3. **للمسؤول:**
   - تسجيل دخول → لوحة التحكم → مراقبة جميع العمليات → إدارة المستخدمين والطلبات

---

## 📁 هيكل المشروع

```
Blood Donation/
├── Controllers/           # Controllers (MVC)
│   ├── AccountController.cs
│   ├── BloodRequestController.cs
│   ├── DonorController.cs
│   ├── AdminController.cs
│   └── MatchingController.cs
├── Models/               # Data Models
│   ├── User.cs
│   ├── Donor.cs
│   ├── Patient.cs
│   ├── BloodRequest.cs
│   └── DonationMatch.cs
├── Views/                # Razor Views
│   ├── Account/
│   ├── BloodRequest/
│   ├── Donor/
│   ├── Admin/
│   └── Shared/
├── Services/             # Business Logic
│   └── BloodCompatibilityService.cs
├── Infrastructure/       # Database Context
│   └── ApplicationDbContext.cs
├── Migrations/           # EF Migrations
├── wwwroot/              # Static Files
│   ├── css/
│   ├── js/
│   └── lib/
└── Program.cs            # Entry Point
```

---

## 🎨 لقطات الشاشة

### الصفحة الرئيسية
- واجهة ترحيبية للزوار
- محتوى ديناميكي للمستخدمين المسجلين
- إحصائيات وأرقام

### لوحة التحكم
- إحصائيات شاملة
- إدارة المستخدمين
- مراقبة الطلبات

### طلبات التبرع
- بطاقات جميلة لكل طلب
- فلاتر بحث متقدمة
- حالات عاجلة مميزة

---

## 🔄 Updates & Commits

المشروع منظم في commits واضحة:
1. ✅ Project Setup & Models
2. ✅ Authentication System
3. ✅ Modern UI Design
4. ✅ Smart Homepage
5. ✅ Blood Request System
6. ✅ Donor Features
7. ✅ Matching Algorithm
8. ✅ Admin Dashboard
9. ✅ Configuration Files

---

## 🤝 المساهمة

نرحب بالمساهمات! إذا كنت ترغب في المساهمة:

1. Fork المشروع
2. أنشئ Branch جديد (`git checkout -b feature/AmazingFeature`)
3. Commit التغييرات (`git commit -m 'Add some AmazingFeature'`)
4. Push للـ Branch (`git push origin feature/AmazingFeature`)
5. افتح Pull Request

---

## 📝 الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE)

---

## 👨‍💻 المطور

**Khaled Salem**

- GitHub: [@KhaledSalem4](https://github.com/KhaledSalem4)

---

## 📧 التواصل

لأي استفسارات أو اقتراحات:
- افتح Issue في GitHub
- تواصل عبر البريد الإلكتروني

---

<div align="center">

### ⭐ إذا أعجبك المشروع، لا تنسَ إعطائه نجمة!

**صُنع بـ ❤️ لإنقاذ الأرواح**

</div>
