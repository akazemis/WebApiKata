using WebApiKata.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiKata.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetSortedProducts(SortType sortType);
    }
}
