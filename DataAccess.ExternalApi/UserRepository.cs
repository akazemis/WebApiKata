using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace DataAccess.ExternalApi
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfigProvider _configProvider;

        public UserRepository(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        /// <summary>
        /// Get default user
        /// </summary>
        /// <remarks>
        /// This method doesn't have to be async but it's made async to be able to retreive it from other resources such as DB or APIs
        /// </remarks>
        /// <returns></returns>
        public async Task<User> GetUser()
        {
            var token = _configProvider.GetConfigValue(ConfigKeys.Token);
            return new User() { Name = "test", Token = token };
        }
    }
}
