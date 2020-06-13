using System.ComponentModel.DataAnnotations;

namespace Cross.Abstractions.EntityEnums
{
    public enum RoleTypes : byte
    {
        [Display(Name = "Admin")]
        Admin = 1,
        [Display(Name = "User")]
        User = 2,
        [Display(Name = "Developer Support")]
        DeveloperSupport = 3
    }
}
