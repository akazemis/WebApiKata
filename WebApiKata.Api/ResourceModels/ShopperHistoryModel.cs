using System.Collections.Generic;

namespace WebApiKata.Api.ResourceModels
{
    public class ShopperHistoryModel
    {
        public int CustomerId { get; set; }

        public List<ProductModel> Products { get; set; }
    }
}
