using WebApiKata.Models;
using System.Threading.Tasks;

namespace WebApiKata.Interfaces
{
    public interface ITrolleyCalculator
    {
        Task<decimal> CalculateTrolley(TrolleyInfo trolleyInfo);
    }
}
