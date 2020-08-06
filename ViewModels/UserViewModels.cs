using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public abstract class UserBaseViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "آدرس ایمیل")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)(|-deleted\d{4})$", ErrorMessage = "ایمیل وارد شده داری فرمت نامعتبر می باشد")]
        public virtual string EmailAddress { get; set; }
    }

    public class EmailViewModel : UserBaseViewModel { }

    public class AddUserViewModel : UserBaseViewModel
    {

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string Password { get; set; }

        [Required]
        public int ActivationCode { get; set; }
    }

    public class UserSignInViewModel : ListUserViewModel
    {
        public string SerialNumber { get; set; }
    }

    public class EditUserViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Name")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "{0} must be somewhat between {1} and {2} characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "عکس پروفایل")]
        [DataType(DataType.ImageUrl)]
        public virtual string Picture { get; set; }

        [Display(Name = "Bio")]
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class ListUserViewModel : UserBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Name")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "{0} must be somewhat between {1} and {2} characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "فعال")]
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "عکس پروفایل")]
        [DataType(DataType.ImageUrl)]
        public virtual string Picture { get; set; }

        public DateTimeOffset? LastLoggedIn { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class ListMerchantViewModel : ListUserViewModel { }

    public class ListTransporterViewModel : ListUserViewModel { }

    public class DetailUserViewModel : ListUserViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "Bio")]
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
    }

    public class UserResetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور جدید")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تکرار کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        [Compare(nameof(NewPassword), ErrorMessage = "{0} نامعتبر می‌باشد.")]
        public string PasswordConfirm { get; set; }
    }

    public class UserChangePasswordViewModel : UserResetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور قدیمی")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string OldPassword { get; set; }
    }

    public class SignInInfoViewModel : UserBaseViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class ActivationCodeViewModel : UserBaseViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "ActivationCode")]
        //[RegularExpression(@"^\d$", ErrorMessage = "Enter A Valid Activation Code")]
        public int ActivationCode { get; set; }
    }

    public class UserRegisterViewModel : UserBaseViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Name")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "{0} must be somewhat between {1} and {2} characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} can accept a maximum of 128 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "PasswordConfirm")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} can accept a maximum of 128 characters")]
        [Compare(nameof(Password), ErrorMessage = "{0} doesn't match")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Role Required")]
        public bool Role { get; set; }
    }

    public class AddMerchantViewModel
    {
        public int MerchantId { get; set; }
    }    
    
    public class AddTransporterViewModel
    {
        public int TransporterId { get; set; }
    }

    public class UserForgetPasswordViewModel : EmailViewModel { }

    public class UploadedPhotoViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "Picture")]
        [DataType(DataType.ImageUrl)]
        public string Picture { get; set; }
    }
}
