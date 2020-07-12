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
}
