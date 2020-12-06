using DataAccess.Models;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ITrolleyRepository
    {
        Task<double> CalculateTrolley(TrolleyInfo trolleyInfo);
    }
}
