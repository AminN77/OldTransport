using Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model
{
    public class Offer : IOffer
    {
        public Offer()
        {

        }

        [ForeignKey("Transporter")]
        public int TransporterId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        public int EstimatedTime { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public virtual Transporter Transporter { get; set; }

        public virtual Project Project { get; set; }
    }
}
