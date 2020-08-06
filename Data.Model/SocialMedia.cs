using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class SocialMedia : ISocialMedia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Link { get; set; }

        [ForeignKey("Settings")]
        public int SettingsId { get; set; }

        public Settings Settings { get; set; }
    }
}
