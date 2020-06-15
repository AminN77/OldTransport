using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Abstractions.Models;

namespace Data.Model
{
    public class Merchant : IMerchant
    {
        public Merchant()
        {

        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string Bio { get; set; }

        public User User { get; set; }

        public virtual ICollection<Accept> Accepts { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
