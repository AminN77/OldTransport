using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class SettingsViewModels
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

        [DataType(DataType.MultilineText)]
        public string HowItWorks { get; set; }

        [DataType(DataType.MultilineText)]
        public string TermsAndConditions { get; set; }
    }

    public class IndexSettingsViewModel
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

    public class HowItWorksViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }

    public class EditHowItWorksViewModel : HowItWorksViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class ListHowItWorksViewModel : EditHowItWorksViewModel { }

    public class TermsAndConditionsViewModel
    {
        [DataType(DataType.MultilineText)]
        public string TermsAndConditions { get; set; }
    }
}
