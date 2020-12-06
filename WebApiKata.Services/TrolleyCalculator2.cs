using WebApiKata.Services.Extensions;
using WebApiKata.Interfaces;
using WebApiKata.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace WebApiKata.Services
{
    public class TrolleyCalculator2 : ITrolleyCalculator
    {
        public async Task<decimal> CalculateTrolley(TrolleyInfo trolleyInfo)
        {
            return GetMinPrice(trolleyInfo);
        }

        public decimal GetMinPrice(TrolleyInfo trolleyInfo)
        {
            if (trolleyInfo == null)
            {
                return 0;
            }
            if(TrolleyInfoPropertiesAreNull(trolleyInfo))
            {
                throw new ArgumentException("Products, Quantities and Specials properties cannot be null.");
            }

            var maxPrice = GetMaxPrice(trolleyInfo);

            if (maxPrice == 0)
            {
                return 0;
            }

            var specials = GetApplicableSpecials(trolleyInfo, maxPrice);
            var shoppedProducts = GetShoppedProducts(trolleyInfo);
            var priceMap = GetProductPriceMap(trolleyInfo);

            var minPrice = maxPrice;
            var i = 0;
            var minPriceUpdated = false;
            if(!specials.Any())
            {
                return minPrice;
            }
            while (!minPriceUpdated)
            {
                minPriceUpdated = false;
                for (var j = 0; j < specials.Count; ++j)
                {
                    GetMinPrice(priceMap, maxPrice, specials, shoppedProducts, ref minPrice, i, ref minPriceUpdated, j);
                }

                i++;
            }

            return minPrice;
        }

        private bool TrolleyInfoPropertiesAreNull(TrolleyInfo trolleyInfo)
        {
            return trolleyInfo.Products == null ||
                    trolleyInfo.Quantities == null ||
                    trolleyInfo.Specials == null;
        }

        private void GetMinPrice(Dictionary<string,decimal> productPriceMap,  decimal maxPrice, List<TrolleySpecial> specials, Dictionary<string, int> shoppedProducts, ref decimal minPrice, int i, ref bool minPriceUpdated, int j)
        {
            var specialCounts = new List<int>();
            specialCounts.InitList(i, specials.Count);
            var hasMore = true;
            for (var k = j; hasMore; ++k)
            {
                int oldValue = specialCounts[j];
                specialCounts[j] = oldValue + k;
                if (SpecialIsApplicable(shoppedProducts, specials, specialCounts))
                {
                    var priceForSpecials = GetPriceForSpecials(specials, specialCounts);
                    if (maxPrice > priceForSpecials)
                    {
                        var priceForRest = GetPriceForNonSpecials(productPriceMap, specials, specialCounts, shoppedProducts);
                        var price = priceForSpecials + priceForRest;
                        if (price < minPrice)
                        {
                            minPrice = price;
                            minPriceUpdated = true;
                        }

                        specialCounts[j] = oldValue;
                    }
                }
                else
                {
                    hasMore = false;
                }
            }
        }

        private bool SpecialIsApplicable(Dictionary<string, int> shoppedProducts, List<TrolleySpecial> specials, List<int> specialCounts)
        {
            foreach (KeyValuePair<string, int> entry in shoppedProducts)
            {
                shoppedProducts.TryGetValue(entry.Key, out int maxCount);
                long count = 0;
                int idx = 0;
                foreach (TrolleySpecial special in specials)
                {
                    var product = special.Quantities.FirstOrDefault(p => p.Name == entry.Key);
                    if (product != null)
                    {
                        count += specialCounts[idx] * product.Quantity;
                    }

                    idx++;
                }

                if (count > maxCount)
                {
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, int> GetShoppedProducts(TrolleyInfo trolleyInfo)
        {
            return trolleyInfo.Quantities.ToDictionary(q => q.Name, q => q.Quantity);
        }

        private Dictionary<string, decimal> GetProductPriceMap(TrolleyInfo trolleyInfo)
        {
            return trolleyInfo.Products.ToDictionary(q => q.Name, q => q.Price);
        }

        private decimal GetPriceForNonSpecials(Dictionary<string,decimal> productPriceMap, List<TrolleySpecial> specials, List<int> specialCounts, Dictionary<string, int> shoppedProducts)
        {
            decimal result = 0;
            foreach (KeyValuePair<string, int> entry in shoppedProducts)
            {
                decimal productPrice = GetProductPrice(productPriceMap, entry.Key);
                result += (entry.Value - GetQuantityByProduct(specials, specialCounts, entry.Key)) * productPrice;
            }

            return result;
        }

        private decimal GetProductPrice(Dictionary<string,decimal> productPriceMap, string productName)
        {
            return productPriceMap.ContainsKey(productName)
                   ? productPriceMap[productName]
                   : 0;
        }

        private int GetQuantityByProduct(List<TrolleySpecial> specials, List<int> specialCounts, string productName)
        {
            int quantity = 0;
            int idx = 0;
            foreach (TrolleySpecial special in specials)
            {
                var product = special.Quantities.FirstOrDefault(p => p.Name == productName);
                if (product != null)
                {
                    quantity += product.Quantity * specialCounts[idx];
                }
                idx++;
            }

            return quantity;
        }

        private List<TrolleySpecial> GetApplicableSpecials(TrolleyInfo input, decimal maxPrice)
        {
            // The price is less than the max price when buying the individual items.
            var result = input.Specials.Where(s => s.Total < maxPrice).ToList();

            // Should not to have any products that are not in the shopping cart.
            var shoppedProductNames = input.Quantities.Select(q => q.Name).ToList();
            result = result.Where(s => s.Quantities.All(q => shoppedProductNames.Contains(q.Name))).ToList();

            // Should not to have more products than the requested quantity.
            var shoppedProductQuntities = input.Quantities.ToDictionary(q => q.Name, q => q.Quantity);
            return result.Where(s => s.Quantities.All(q => { shoppedProductQuntities.TryGetValue(q.Name, out int v); return v >= q.Quantity; })).ToList();
        }

        private decimal GetPriceForSpecials(List<TrolleySpecial> specials, List<int> specialCounts)
        {
            decimal result = 0;
            for (int i = 0; i < specialCounts.Count; i++)
            {
                result += specialCounts[i] * specials[i].Total;
            }

            return result;
        }

        private decimal GetMaxPrice(TrolleyInfo trolleyInfo)
        {
            decimal price = 0;
            foreach (TrolleyQuantity quantity in trolleyInfo.Quantities)
            {
                foreach (TrolleyProduct product in trolleyInfo.Products)
                {
                    if (product.Name == quantity.Name)
                    {
                        price += quantity.Quantity * product.Price;
                        break;
                    }
                }
            }

            return price;
        }
    }
}
