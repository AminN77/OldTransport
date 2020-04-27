using System.ComponentModel.DataAnnotations;
namespace Cross.Abstractions.EntityEnums
{
    public enum RoleType : byte
    {
        [Display(Name = "توسعه دهنده")]
        DeveloperSupport,
        [Display(Name = "مدیر")]
        Admin,
        [Display(Name = "شرکت حمل و نقل")]
        TransportCompany,
        [Display(Name = "بازرگان")]
        Merchant
    }
}
