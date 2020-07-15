using System.ComponentModel.DataAnnotations;

namespace Cross.Abstractions.EntityEnums
{
    public enum AcceptStatus : byte
    {
        [Display(Name = "Loading")]
        Loading = 0,
        [Display(Name = "Shipping")]
        Shipping = 1,
        [Display(Name = "Delivered")]
        Delivered = 2,
        [Display(Name = "Canceled By Transporter")]
        TCanceled = 3,
        [Display(Name = "Canceled By Merchant")]
        Mcanceled = 4
    }
}
