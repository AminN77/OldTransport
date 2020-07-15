using Cross.Abstractions.EntityEnums;
using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public AcceptStatus Status { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual Merchant Merchant { get; set; }
    }
}
