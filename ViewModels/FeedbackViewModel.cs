using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class SendFeedbackViewModel
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }

    public class AdminCheckFeedback { }
}
