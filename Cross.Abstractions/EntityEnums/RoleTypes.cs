using System.ComponentModel.DataAnnotations;

namespace Cross.Abstractions.EntityEnums
{
    public enum RoleTypes : byte
    {
        [Display(Name = "مدیر")]
        Admin = 1,
        [Display(Name = "کاربر")]
        User = 2,
        [Display(Name = "مدیریت تنظیمات")]
        DeveloperSupport = 3
    }
}
