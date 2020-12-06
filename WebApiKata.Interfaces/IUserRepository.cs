using WebApiKata.Models;
using System;
using System.Threading.Tasks;

namespace WebApiKata.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser();
    }
}
