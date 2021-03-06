using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class UserRole
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
