using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Abstractions.Models;

namespace Data.Model
{
    public class User: IUser
    {
        public User()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        [Required]
        [RegularExpression(@"^\w{1,64}(|-deleted\d{4})$")]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "char(128)")]
        [StringLength(128, MinimumLength = 128)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128, MinimumLength = 128)]
        public string EmailAddress { get; set; }

        [StringLength(128, MinimumLength = 1)]
        public string Picture { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        [MaxLength(32)]
        [MinLength(32)]
        public byte[] Salt { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IterationCount { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

    }
}