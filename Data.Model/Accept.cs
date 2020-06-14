using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model
{
    public class Accept
    {
        public Accept()
        {

        }

        [ForeignKey("Transporter")]
        public int TransporterId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }        
        
        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }        
        
        [ForeignKey("Offer")]
        public int OfferId { get; set; }

        public virtual Transporter Transporter { get; set; }

        public virtual Project Project { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual Merchant Merchant { get; set; }
    }
}
