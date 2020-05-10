using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Data.Abstractions.Models;

namespace Data.Model
{
    public class Role : IRole
    {
        public Role()
        {

        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1)]
        public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
