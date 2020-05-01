using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public abstract class UserBaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نام")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر ‌باشد.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نام خانوادگی")]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر ‌باشد.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نام کاربری")]
        [RegularExpression(@"^\w{1,64}$",ErrorMessage = "{0} باید {1} کاراکتر ‌باشد.")]
        public virtual string Username { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "فعال")]
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "آدرس ایمیل")]
        [DataType(DataType.EmailAddress)]
        public virtual string EmailAddress { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "عکس پروفایل")]
        [DataType(DataType.ImageUrl)]
        public virtual string Picture { get; set; }

    }

    public class AddUserViewModel : EditUserViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تکرار کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        [Compare(nameof(Password), ErrorMessage = "{0} نامعتبر می‌باشد.")]
        public string PasswordConfirm { get; set; }
    }

    public class UserSignInViewModel:UserBaseViewModel
    {
        [Required]
        public string Token { get; set; }
    }

    public class EditUserViewModel : UserBaseViewModel
    {
    }

    public class ListUserViewModel : UserBaseViewModel
    {
    }

    public class DetailUserViewModel : ListUserViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDateTime { get; set; }
    }

    public class UserSetPasswordViewModel
    {
        public int Id { get; set; }

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

    public class UserChangePasswordViewModel : UserSetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور قدیمی")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string OldPassword { get; set; }
    }

    public class SignInInfoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "نام کاربری")]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر ‌باشد.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [Display(Name = "کلمه عبور")]
        [DataType(DataType.Password)]
        [MaxLength(128, ErrorMessage = "{0} حداکثر {1} کاراکتر می‌باشد.")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
