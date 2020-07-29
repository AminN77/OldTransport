namespace Data.Abstractions.Models
{
    public interface ISettings : IEntity<int>
    {
        string ContactEmail { get; set; }

        string AboutUs { get; set; }

        string Logo { get; set; }

        string ContactNumber { get; set; }

        string TermsAndConditions { get; set; }

        int OffersCountLimit { get; set; }
    }
}
