using System.ComponentModel.DataAnnotations;

namespace Cross.Abstractions.EntityEnums
{
    public enum AcceptStatus : byte
    {
        [Display(Name = "Accepted")]
        Accepted = 0,
        [Display(Name = "Loading")]
        Loading = 1,
        [Display(Name = "Shipping")]
        Shipping = 2,
        [Display(Name = "Delivered")]
        Delivered = 3,
        [Display(Name = "Canceled By Transporter")]
        TCanceled = 4,
        [Display(Name = "Canceled By Merchant")]
        Mcanceled = 5
    }
}
