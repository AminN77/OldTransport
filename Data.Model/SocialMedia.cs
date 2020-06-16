using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class SocialMedia : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Link { get; set; }
    }
}
