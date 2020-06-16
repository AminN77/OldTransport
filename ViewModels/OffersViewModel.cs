using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ViewModels
{
    public abstract class BaseOfferViewModel
    {
        public int TransporterId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "please enter {0}")]
        public string Description { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "please enter {0}")]
        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        [Display(Name = "Estimated Time (Days)")]
        [Required(ErrorMessage = "please enter {0}")]
        public int EstimatedTime { get; set; }
    }

    public class AddOfferViewModel : BaseOfferViewModel
    {
    }

    public class ListOfferViewModel : EditOfferViewModel
    {
        public string TransporterName { get; set; }
    }

    public class EditOfferViewModel : BaseOfferViewModel
    {
        [Required]
        public int offerId { get; set; }
    }

    public class OfferDetailsViewModel : ListOfferViewModel
    {
    }
}
