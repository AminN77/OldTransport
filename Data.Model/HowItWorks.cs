using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class HowItWorks : IHowItWorks
    {
        public HowItWorks()
        {

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [ForeignKey("Settings")]
        public int SettingsId { get; set; }

        public Settings Settings { get; set; }
    }
}
