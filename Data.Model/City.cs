using Data.Abstractions.Models;

namespace Data.Model
{
    public class City : ICity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
