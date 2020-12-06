using WebApiKata.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using WebApiKata.Api.Config;

namespace WebApiKata.Api.Providers
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly ConcurrentDictionary<string, string> _configDictionary;
        private readonly IOptions<ExternalApiSettings> _externalApiSettings;

        public ConfigProvider(IOptions<ExternalApiSettings> externalApiSettings)
        {
            _configDictionary = new ConcurrentDictionary<string, string>();
            _externalApiSettings = externalApiSettings;
            LoadConfig();
        }

        public string GetConfigValue(string configKey)
        {
            ValidateConfigKey(configKey);
            return _configDictionary[configKey];
        }

        private void ValidateConfigKey(string configKey)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentException("ConfigKey cannot be null or empty or only whitespace.");
            }
            if (!_configDictionary.ContainsKey(configKey))
            {
                throw new ArgumentException($"ConfigKey '{configKey}' is not valid.");
            }
        }

        private void LoadConfig()
        {
            _configDictionary[ConfigKeys.Token] = _externalApiSettings?.Value?.Token;
            _configDictionary[ConfigKeys.BaseUrl] = _externalApiSettings?.Value?.BaseUrl;
        }
    }
}
