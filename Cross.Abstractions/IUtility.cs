namespace Cross.Abstractions
{
    public interface IUtility
    {
        string[] GetEntityProperties(object entity, params string[] exceptProperties);
    }
}
