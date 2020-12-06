using System.Collections.Generic;

namespace DataAccess.Models
{
    public class TrolleyInfo
    {
        public List<TrolleyProduct> Products { get; set; } = new List<TrolleyProduct>();
        public List<TrolleySpecial> Specials { get; set; } = new List<TrolleySpecial>();
        public List<TrolleyProduct> Quantities { get; set; } = new List<TrolleyProduct>();


        public class TrolleyProduct
        {
            public string Name { get; set; }

            public double Price { get; set; }
        }

        public class TrolleySpecial
        {
            public List<TrolleyQuantity> Quantities { get; set; }

            public double Total { get; set; }
        }

        public class TrolleyQuantity
        {
            public string Name { get; set; }

            public double Quantity { get; set; }
        }
    }
}
