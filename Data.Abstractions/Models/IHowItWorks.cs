namespace Data.Abstractions.Models
{
    public interface IHowItWorks : IEntity<int>
    {
        string Name { get; set; }

        string Text { get; set; }
    }
}
