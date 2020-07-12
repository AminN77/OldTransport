using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ViewModels
{
    public class SendFeedbackViewModel
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}
