using System;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public abstract class BaseOfferViewModel
    {
        public int Id { get; set; }
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

    public class ListOfferViewModel : BaseOfferViewModel
    {
        public string TransporterName { get; set; }
        public string ProjectName { get; set; }
        public bool IsAccepted { get; set; }
    }

    public class EditOfferViewModel : BaseOfferViewModel { }

    public class OfferDetailsViewModel : ListOfferViewModel
    {
        public int MerchantId { get; set; }

        public int AcceptId { get; set; }
    }

    public class DeleteOfferViewModel : EditOfferViewModel
    {
        public bool IsDeleted { get; set; }
    }
}
