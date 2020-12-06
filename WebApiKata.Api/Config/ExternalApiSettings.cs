namespace WebApiKata.Api.Config
{
    /// <summary>
    /// Holds info from the corresponding ExternalApiSettings of the appsettings.json
    /// </summary>
    public class ExternalApiSettings
    {
        public string BaseUrl { get; set; }
        public string Token { get; set; }
    }
}
