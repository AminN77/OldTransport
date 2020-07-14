using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class SendFeedbackViewModel
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }

    public class AdminCheckFeedbackViewModel
    {
        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime CreateDateTime { get; set; }

        public int UserId { get; set; }
    }

    public class FeedbackListViewModel : AdminCheckFeedbackViewModel
    {
        public int Id { get; set; }
    }

    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Email Address")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)(|-deleted\d{4})$", ErrorMessage = "Invalid Email Format")]
        public virtual string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "Name")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "{0} must be somewhat between {1} and {2} characters")]
        public string Name { get; set; }
    }

    public class ContactMessagesListViewModel : ContactUsViewModel
    {
        public int id { get; set; }
    }

    public class ContactMessageViewModel : ContactMessagesListViewModel { }
}
