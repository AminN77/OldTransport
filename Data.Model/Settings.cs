using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Settings: IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
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
