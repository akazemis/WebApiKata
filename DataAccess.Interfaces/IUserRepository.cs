using DataAccess.Models;
using System;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser();
    }
}
