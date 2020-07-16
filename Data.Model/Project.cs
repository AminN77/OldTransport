using Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model
{
    public class Project : IProject
    {
        public Project()
        {

        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        public string BeginningCountry { get; set; }

        [Required]
        public string BeginningCity { get; set; }

        [Required]
        public string DestinationCountry { get; set; }

        [Required]
        public string DestinationCity { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Cargo { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Budget { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Weight { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Dimention { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public double Quantity { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public virtual Merchant Merchant { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }
    }
}
