using System.ComponentModel.DataAnnotations;
namespace Cross.Abstractions.EntityEnums
{
    public enum RoleType : byte
    {
        [Display(Name = "مدیر")]
        Admin,
        [Display(Name = "بازرگان")]
        Merchant,
        [Display(Name = "شرکت حمل و نقل")]
        Transporter,
        [Display(Name = "توسعه دهنده")]
        DeveloperSupport,
    }
}
