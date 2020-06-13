using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ViewModels
{
    public abstract class ProjectBaseViewModel
    {

        [Display(Name = "Beginning")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string Beginning { get; set; }

        [Display(Name = "Destination")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string Destination { get; set; }

    }

    public class AddProjectViewModel : ProjectBaseViewModel
    {

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }



        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please enter {0}")]
        [MaxLength(500)]
        public string Title { get; set; }


        [Display(Name = "Budget")]
        [Required(ErrorMessage = "Please enter {0}")]
        [Range(0.0, double.MaxValue)]
        public double Budget { get; set; }

        [Display(Name = "Weight")]
        [Required(ErrorMessage = "Please enter {0}")]
        [Range(0.0, double.MaxValue)]
        public double Weight { get; set; }
    }

    public class ListProjectViewModel : ProjectBaseViewModel
    {

    }

    public class EditProjectViewModel : ProjectBaseViewModel
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeleteProjectViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
