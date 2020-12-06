namespace WebApiKata.Interfaces
{
    public interface IConfigProvider
    {
        string GetConfigValue(string configKey);
    }
}
