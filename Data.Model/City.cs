using Data.Abstractions.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class City : ICity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public Country Country { get; set; }
    }
}
