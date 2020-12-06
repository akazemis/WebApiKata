using WebApiKata.Models;
using System;
using System.Threading.Tasks;

namespace WebApiKata.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser();
    }
}
