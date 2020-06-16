using Data.Abstractions.Models;
using System.Collections.Generic;

namespace Data.Model
{
    public class Country : ICountry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
