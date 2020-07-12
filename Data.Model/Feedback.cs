using Data.Abstractions.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class Feedback : IFeedback
    {
        public Feedback()
        {

        }

        [Key]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime CreateDateTime { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
