using DataAccess.Interfaces;
using System;

namespace DataAccess.ExternalApi
{
    public class ExternalApiPathProvider : IExternalApiPathProvider
    {
        private readonly string _baseUrl;

        public ExternalApiPathProvider(IConfigProvider configProvider)
        {
            _baseUrl = configProvider.GetConfigValue(ConfigKeys.BaseUrl);
        }

        public string GetApiPath(ExternalApiPathName apiPathName)
        {
            return $"{_baseUrl.TrimEnd('/')}/{GetRelativePath(apiPathName).TrimStart('/')}";
        }

        private string GetRelativePath(ExternalApiPathName apiPathName)
        {
            switch (apiPathName)
            {
                case ExternalApiPathName.GetProducts:
                    return "/api/resource/products";
                case ExternalApiPathName.GetShopperHistory:
                    return "/api/resource/shopperhistory";
                case ExternalApiPathName.CalculateTrolley:
                    return "/api/resource/trolleycalculator";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
