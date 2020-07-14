using System;
using System.Collections.Generic;
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
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "char(128)")]
        [StringLength(128, MinimumLength = 128)]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)(|-deleted\d{4})$")]
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

        public int ActivationCode { get; set; }

        /// <summary>
        /// every time the user changes his Password,
        /// or an admin changes his Roles or stat/IsActive,
        /// create a new `SerialNumber` GUID and store it in the DB.
        /// </summary>
        public string SerialNumber { get; set; }

        public DateTimeOffset? LastLoggedIn { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        public string PhoneNumber { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }
    }
}