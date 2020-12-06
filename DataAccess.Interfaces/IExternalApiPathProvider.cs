namespace DataAccess.Interfaces
{
    public interface IExternalApiPathProvider
    {
        string GetApiPath(ExternalApiPathName apiPathName);
    }
}
