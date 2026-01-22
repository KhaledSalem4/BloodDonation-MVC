namespace Blood_Donation.Services
{
    public class BloodCompatibilityService
    {
        // Dictionary يحدد من يمكنه التبرع لمن
        private static readonly Dictionary<string, List<string>> CompatibilityRules = new()
        {
            // A+ can receive from: A+, A-, O+, O-
            { "A+", new List<string> { "A+", "A-", "O+", "O-" } },
            
            // A- can receive from: A-, O-
            { "A-", new List<string> { "A-", "O-" } },
            
            // B+ can receive from: B+, B-, O+, O-
            { "B+", new List<string> { "B+", "B-", "O+", "O-" } },
            
            // B- can receive from: B-, O-
            { "B-", new List<string> { "B-", "O-" } },
            
            // AB+ can receive from all (Universal receiver)
            { "AB+", new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" } },
            
            // AB- can receive from: A-, B-, AB-, O-
            { "AB-", new List<string> { "A-", "B-", "AB-", "O-" } },
            
            // O+ can receive from: O+, O-
            { "O+", new List<string> { "O+", "O-" } },
            
            // O- can receive from: O- only (Universal donor for giving)
            { "O-", new List<string> { "O-" } }
        };

        /// <summary>
        /// يتحقق ما إذا كان المتبرع يمكنه التبرع للمريض
        /// </summary>
        /// <param name="donorBloodType">فصيلة دم المتبرع</param>
        /// <param name="recipientBloodType">فصيلة دم المستقبل (المريض)</param>
        /// <returns>true إذا كان التبرع متوافق</returns>
        public static bool IsCompatible(string donorBloodType, string recipientBloodType)
        {
            if (string.IsNullOrEmpty(donorBloodType) || string.IsNullOrEmpty(recipientBloodType))
                return false;

            if (!CompatibilityRules.ContainsKey(recipientBloodType))
                return false;

            return CompatibilityRules[recipientBloodType].Contains(donorBloodType);
        }

        /// <summary>
        /// يحصل على قائمة فصائل الدم المتوافقة للتبرع لفصيلة دم معينة
        /// </summary>
        /// <param name="recipientBloodType">فصيلة دم المستقبل</param>
        /// <returns>قائمة بفصائل الدم المتوافقة</returns>
        public static List<string> GetCompatibleDonorTypes(string recipientBloodType)
        {
            if (string.IsNullOrEmpty(recipientBloodType) || !CompatibilityRules.ContainsKey(recipientBloodType))
                return new List<string>();

            return CompatibilityRules[recipientBloodType];
        }

        /// <summary>
        /// يحصل على رسالة توضيحية للتوافق
        /// </summary>
        public static string GetCompatibilityMessage(string donorBloodType, string recipientBloodType)
        {
            if (IsCompatible(donorBloodType, recipientBloodType))
            {
                if (donorBloodType == recipientBloodType)
                    return "تطابق مثالي - نفس فصيلة الدم";
                else if (donorBloodType == "O-")
                    return "متوافق - O- متبرع عام لكل الفصائل";
                else if (recipientBloodType == "AB+")
                    return "متوافق - AB+ مستقبل عام من كل الفصائل";
                else
                    return "متوافق - فصائل الدم متطابقة طبياً";
            }
            return "غير متوافق - لا يمكن التبرع";
        }
    }
}
