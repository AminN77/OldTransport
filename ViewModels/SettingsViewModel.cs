using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Please Enter {0}")]
        [Display(Name = "Contact Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)(|-deleted\d{4})$", ErrorMessage = "Invaild Email address")]
        public string ContactEmail { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string AboutUs { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        public string ContactNumber { get; set; }
    }

    public class UserGuideViewModel
    {
        [DataType(DataType.MultilineText)]
        public string UserGuide { get; set; }
    }

    public class TermsAndConditionsViewModel
    {
        [DataType(DataType.MultilineText)]
        public string TermsAndConditions { get; set; }
    }
}
