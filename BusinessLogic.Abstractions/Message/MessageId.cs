using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Abstractions.Message
{
    public enum MessageId
    {
        [Display(Name = "خطایی رخ داده است. لطفا با مدیر سیستم تماس حاصل فرمایید.")]
        Exception,
        [Display(Name = "خطای {0} رخ داده. لطفا دوباره تلاش کنید در صورت مواجه‌ی دوباره با خطا با پشتیبانی تماس حاصل فرمایید.")]
        InternalError,
        [Display(Name = "خطای {0} رخ داده. لطفا با مدیر سیستم تماس حاصل فرمایید.")]
        DataInconsistency,
        [Display(Name = "{0} موجود نمی‌باشد.")]
        EntityDoesNotExist,
        [Display(Name = "نام کاربری {0} تکراری است. لطفا نام کاربری دیگری انتخاب کنید.")]
        UsernameAlreadyExisted,
        [Display(Name = "کاربر با موفقیت ثبت شد.")]
        UserSuccessfullyAdded,
        [Display(Name = "فیلدهای رمز عبور یکسان نمی‌باشد. دوباره وارد شود.")]
        PasswordAndPasswordConfirmAreNotMached,
        [Display(Name = "کاربر با موفقیت ویرایش شد.")]
        UserSuccessfullyEdited,
        [Display(Name = "کاربر با موفقیت حذف شد.")]
        UserSuccessfullyDeleted,
        [Display(Name = "Access Denied")]
        AccessDenied,
        [Display(Name = "پسورد با موفقیت تغییر یافت.")]
        PasswordSuccessfullyChanged,
        [Display(Name = " پسورد با موفقیت بازیابی شد.")]
        PasswordSuccessfullyReseted,
        [Display(Name = "کاربر {0} با موفقیت {1} شد.")]
        UserIsEnabledSuccessfullySet,
        [Display(Name = "{0} معتبر نمی‌باشد.")]
        InvalidInput,
        [Display(Name = "رمزعبور وارد شده صحیح نمی‌باشد.")]
        InvalidPassword,
        [Display(Name = "رمزعبور و/یا شناسه کاربری وارد شده صحیح نمی‌باشد.")]
        UsernameOrPasswordInvalid,
        BothOfThesePropertiesMustHaveValue,
        TimeSpanMustBeBetweenZeroAndTwentyFourHours,
        ThisPropertyMustBeGreaterThanThatProperty,
        ThisPropertyMustBeLowerThanThatProperty,
        [Display(Name = "تغییرات با موفقیت ذخیره شد.")]
        SettingSuccessfullySaved,
        [Display(Name = "{0} با موفقیت حذف شد.")]
        EntitySuccessfullyDeleted,
        [Display(Name = "Entity Successfully Updated")]
        EntitySuccessfullyUpdated,
        [Display(Name = "{0} با موفقیت افزوده شد.")]
        EntitySuccessfullyAdded,
        Successed,
        // TODO: How to capital case first word?
        [Display(Name = "{0} {1} موجود است. لطفا {0} دیگری انتخاب کنید..")]
        EntitiesFieldsValueAlreadyExisted,
        [Display(Name = "نقش های اصلی قابل تغییر نیستند.")]
        PrimativeRolesCanNotBeDeleted,
        [Display(Name = "نام نقش های اصلی قابل تغییر نیستند.")]
        PrimativeRoleNamesCanNotChanged,
        [Display(Name = "{0} قبلا حذف شده.")]
        EntityWasDeleted,
        [Display(Name = "این {0} دارد تعدادی {1}.")]
        EntityHasRelatedData,
        [Display(Name = "Cannot {0} all SuperAdmin users.")]
        EditLimitationForSuperAdmins,
        [Display(Name = "Admin user cannot {0} SuperAdmin user.")]
        AdminCannotModifySuperAdmin,
        [Display(Name = "This user is assigned to some building as admin. please select another admin for the building(s) before changing role of this user.")]
        ChangeAdminToDistributed,
        [Display(Name = "{0} حذف ناموفق به دلیل رابطه با داده های دیگر.")]
        EntityUnSuccessfullyDelete,
        [Display(Name = "درخواست فرستاده شد برای {0}")]
        UserSuccessfullyInvited,
        [Display(Name = "این سطح سازمانی دارای زیر شاخه است.")]
        ThisOrganizationPostHasChild,
        [Display(Name = "کاربرانی در این سطح سازمانی موجود می‌باشند.")]
        ThisOrganizationPostHasUsers,
        [Display(Name = "EmailDoesNotExist")]
        EmailDoesNotExist,
        [Display(Name = "ایمیل وارد شده قبلا ثبت نام شده است")]
        EmailSuccessfullyVerified,
        [Display(Name = "ایمیل حاوی کد فعالسازی با موفقیت ارسال شد")]
        VerificationEmailSuccessfullySent,
        [Display(Name = "User Successfully Activated")]
        UserSuccessfullyActivated,
        [Display(Name = "Activation Code Is Not Valid")]
        ActivationCodeVerficationFailed,
        [Display(Name = "EmailSendingProcessFailed")]
        EmailSendingProcessFailed,
        [Display(Name = "User Successfully Deactivated")]
        UserSuccessfullyDeactivated,
        [Display(Name = "CanNotSendEmail")]
        CanNotSendEmail,
        [Display(Name = "ProjectSuccessfullyAdded")]
        ProjectSuccessfullyAdded,
        [Display(Name = "ProjectNotFound")]
        ProjectNotFound,
        [Display(Name = "This User Is Not Active")]
        DeactivatedUser,
        [Display(Name = "Invalid File Type")]
        InvalidFileType,
        [Display(Name = "Merchant Exists")]
        MerchantExists,
        [Display(Name = "Transporter Exists")]
        TransporterExists,
        [Display(Name = "Duplicated Offer")]
        DuplicatedOffer,
        [Display(Name = "Maximum Allowed Offers Limit Reached")]
        MaximumAllowedOffers,
        [Display(Name = "Cannot Delete Account Due To Active Project")]
        CannotDeleteAccountDueToActiveProject,
        [Display(Name = "Cannot Delete Active Project")]
        CannotDeleteActiveProject,
        [Display(Name = "Cannot Delete Active Offer")]
        CannotDeleteActiveOffer,
        [Display(Name = "File Successfully Uploaded")]
        FileSuccessfullyUploaded
    }
}
