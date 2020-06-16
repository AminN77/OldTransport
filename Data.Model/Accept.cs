using Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model
{
    public class Accept : IAccept
    {
        public Accept()
        {

        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        [ForeignKey("Offer")]
        public int OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual Merchant Merchant { get; set; }
    }
}
