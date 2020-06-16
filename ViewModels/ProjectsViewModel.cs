using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ViewModels
{
    public abstract class ProjectBaseViewModel
    {
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please enter {0}")]
        [MaxLength(500)]
        public string Title { get; set; }

        [Display(Name = "Beginning Country")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string BeginningCountry { get; set; }

        [Display(Name = "Beginning City")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string BeginningCity { get; set; }

        [Display(Name = "Destination Country")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string DestinationCountry { get; set; }
        
        [Display(Name = "Destination City")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string DestinationCity { get; set; }

        [Display(Name = "Budget")]
        [Required(ErrorMessage = "Please enter {0}")]
        [Range(0, int.MaxValue)]
        public int Budget { get; set; }

    }

    public class AddProjectViewModel : ProjectBaseViewModel
    {

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Weight")]
        [Required(ErrorMessage = "Please enter {0}")]
        [Range(0.0, double.MaxValue)]
        public double Weight { get; set; }
    }

    public class ListProjectViewModel : ProjectBaseViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool IsEnabled{ get; set; }
    }

    public class EditProjectViewModel : AddProjectViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class AcceptOfferViewModel
    {
        public int MerchantId { get; set; }

        [Required]
        public int OfferId { get; set; }
    }

    public class ProjectDetailsViewModel : ListProjectViewModel
    {
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Weight")]
        [Required(ErrorMessage = "Please enter {0}")]
        [Range(0.0, double.MaxValue)]
        public double Weight { get; set; }

        public string MerchantName { get; set; }
    }
}
