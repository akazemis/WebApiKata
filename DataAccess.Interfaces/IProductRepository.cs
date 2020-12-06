using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetSortedProducts(SortType sortType);
    }
}
