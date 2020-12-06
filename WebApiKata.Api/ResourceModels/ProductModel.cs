using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiKata.Api.ResourceModels
{
    public class ProductModel
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
    }
}
