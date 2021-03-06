﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApiKata.Api.ResourceModels
{
    public class TrolleyInfoModel
    {
        [Required]
        public List<TrolleyProductModel> Products { get; set; } = new List<TrolleyProductModel>();

        [Required]
        public List<TrolleySpecialModel> Specials { get; set; } = new List<TrolleySpecialModel>();

        [Required]
        public List<TrolleyQuantityModel> Quantities { get; set; } = new List<TrolleyQuantityModel>();
    }

    public class TrolleyProductModel
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }

    public class TrolleySpecialModel
    {
        public List<TrolleyQuantityModel> Quantities { get; set; }

        public decimal Total { get; set; }
    }

    public class TrolleyQuantityModel
    {
        public string Name { get; set; }

        public int Quantity { get; set; }
    }
}
