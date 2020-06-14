using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class Transporter
    {
        public Transporter()
        {

        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string Bio { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
    }
}
