using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Settings
    {
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string AboutUs { get; set; }

        [Required]
        public string Logo { get; set; }

        [Required]
        public string ContactNumber { get; set; }
    }
}
