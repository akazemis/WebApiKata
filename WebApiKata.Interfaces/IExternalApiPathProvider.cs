namespace WebApiKata.Interfaces
{
    public interface IExternalApiPathProvider
    {
        string GetApiPath(ExternalApiPathName apiPathName);
    }
}
