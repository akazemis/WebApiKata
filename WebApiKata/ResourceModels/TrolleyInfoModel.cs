using System.Collections.Generic;

namespace WebApiKata.ResourceModels
{
    public class TrolleyInfoModel
    {
        public List<TrolleyProductModel> Products { get; set; }
        public List<TrolleySpecialModel> Specials { get; set; }
        public List<TrolleyQuantityModel> Quantities { get; set; }


        public class TrolleyProductModel
        {
            public string Name { get; set; }

            public double Price { get; set; }
        }

        public class TrolleySpecialModel
        {
            public List<TrolleyQuantityModel> Quantities { get; set; }

            public double Total { get; set; }
        }

        public class TrolleyQuantityModel
        {
            public string Name { get; set; }

            public double Quantity { get; set; }
        }
    }
}
