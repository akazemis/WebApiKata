namespace DataAccess.Interfaces
{
    public interface IConfigProvider
    {
        string GetConfigValue(string configKey);
    }
}
