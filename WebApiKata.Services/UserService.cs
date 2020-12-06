using WebApiKata.Interfaces;
using WebApiKata.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace WebApiKata.Services
{
    public class UserService : IUserService
    {
        private readonly IConfigProvider _configProvider;

        public UserService(IConfigProvider configProvider)
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
            return new User() { Name = "Arvin Kardon", Token = token };
        }
    }
}
