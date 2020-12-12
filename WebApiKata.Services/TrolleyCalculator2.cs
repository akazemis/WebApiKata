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
            return await Task.Run(() => GetMinPossibleAmount(trolleyInfo));
        }

        /// <summary>
        /// Explores the trolley info to discover the minimum possible total amount
        /// </summary>
        /// <param name="trolleyInfo"></param>
        /// <returns></returns>
        public decimal GetMinPossibleAmount(TrolleyInfo trolleyInfo)
        {
            if (trolleyInfo == null)
            {
                return 0;
            }
            if (TrolleyInfoPropertiesAreNull(trolleyInfo))
            {
                throw new ArgumentException("Products, Quantities and Specials properties cannot be null.");
            }
            var productPriceMap = GetProductPriceMap(trolleyInfo);
            var shoppedProductQuantitiesMap = GetShoppedProductQuantitiesMap(trolleyInfo);

            var maxAmount = CalculateShoppedProductsAmountByQuantityAndPrice(shoppedProductQuantitiesMap, productPriceMap);

            if (maxAmount == 0)
            {
                return 0;
            }

            var applicableSpecials = GetApplicableSpecials(trolleyInfo.Specials, maxAmount, shoppedProductQuantitiesMap);

            if (!applicableSpecials.Any())
            {
                return maxAmount;
            }

            var minAmount = maxAmount;
            foreach (var specialList in applicableSpecials.GetPermutations(applicableSpecials.Count))
            {
                SetMinAmountAfterApplyingSpecials(productPriceMap, shoppedProductQuantitiesMap, specialList, ref minAmount);
            }
            return minAmount;
        }

        private void SetMinAmountAfterApplyingSpecials(Dictionary<string, decimal> productPriceMap, Dictionary<string, int> shoppedProductQuantitiesMap, IEnumerable<TrolleySpecial> applicableSpecials, ref decimal currentMinAmount)
        {
            decimal amount = 0;

            foreach (var special in applicableSpecials)
            {
                Dictionary<string, int> shoppedProductQuantitiesMapMinusSpecial;
                amount += GetAmountOfAppliedSpecial(special, shoppedProductQuantitiesMap, out shoppedProductQuantitiesMapMinusSpecial);
                shoppedProductQuantitiesMap = shoppedProductQuantitiesMapMinusSpecial;
                if (amount > currentMinAmount)  // If amount exceeds the currentMinAmount we are not interested in the rest of calculation
                {
                    return;
                }
            }
            amount += CalculateShoppedProductsAmountByQuantityAndPrice(shoppedProductQuantitiesMap, productPriceMap);
            if (amount < currentMinAmount)
            {
                currentMinAmount = amount;
            }
        }

        /// <summary>
        /// Applies the special on the trolley and returns the amount of special, also spits out the remaining items after deducting the calculated items (in <see cref="shoppedProductQuantitiesMapMinusSpecial"/>)
        /// </summary>
        /// <param name="trolleySpecial"></param>
        /// <param name="shoppedProductQuantitiesMap"></param>
        /// <param name="shoppedProductQuantitiesMapMinusSpecial">Shopped products quantities after deducting the calculated specialed items</param>
        /// <returns></returns>
        private decimal GetAmountOfAppliedSpecial(TrolleySpecial trolleySpecial, Dictionary<string, int> shoppedProductQuantitiesMap, out Dictionary<string, int> shoppedProductQuantitiesMapMinusSpecial)
        {
            var shoppedProductQuantitiesMapCopy = shoppedProductQuantitiesMap.ToDictionary(q => q.Key, q => q.Value);
            var maxApplicableCount = GetMaxApplicableCount(trolleySpecial, shoppedProductQuantitiesMapCopy);

            decimal totalAmount = 0;
            if (maxApplicableCount == 0)
            {
                shoppedProductQuantitiesMapMinusSpecial = shoppedProductQuantitiesMap;
                return 0m;
            }

            totalAmount += maxApplicableCount * trolleySpecial.Total;

            foreach (var specialQuantity in trolleySpecial.Quantities)
            {
                shoppedProductQuantitiesMapCopy[specialQuantity.Name] -= (maxApplicableCount * specialQuantity.Quantity);
            }

            shoppedProductQuantitiesMapMinusSpecial = shoppedProductQuantitiesMapCopy;
            return totalAmount;
        }

        private decimal CalculateShoppedProductsAmountByQuantityAndPrice(Dictionary<string, int> shoppedProductQuantitiesMap, Dictionary<string, decimal> productPriceMap)
        {
            decimal result = 0;
            foreach (var item in shoppedProductQuantitiesMap)
            {
                var productName = item.Key;
                var quantity = item.Value;
                if (!productPriceMap.ContainsKey(productName))
                {
                    throw new ArgumentException("A Product in quantity list does not exist in the product list.");
                }
                result += (quantity * productPriceMap[productName]);
            }
            return result;
        }

        private int GetMaxApplicableCount(TrolleySpecial trolleySpecial, Dictionary<string, int> shoppedProductQuantitiesMapCopy)
        {
            var maxApplicableCount = int.MaxValue;
            foreach (var specialQuantity in trolleySpecial.Quantities)
            {
                var count = shoppedProductQuantitiesMapCopy[specialQuantity.Name] / specialQuantity.Quantity;
                maxApplicableCount = Math.Min(count, maxApplicableCount);
            }

            return maxApplicableCount;
        }

        private bool TrolleyInfoPropertiesAreNull(TrolleyInfo trolleyInfo)
        {
            return trolleyInfo.Products == null ||
                   trolleyInfo.Quantities == null ||
                   trolleyInfo.Specials == null;
        }

        private Dictionary<string, int> GetShoppedProductQuantitiesMap(TrolleyInfo trolleyInfo)
        {
            var result = new Dictionary<string, int>();
            foreach (var productQuantity in trolleyInfo.Quantities)
            {
                if (result.ContainsKey(productQuantity.Name))
                {
                    result[productQuantity.Name] += productQuantity.Quantity;
                }
                else
                {
                    result[productQuantity.Name] = productQuantity.Quantity;
                }
            }
            return result;
        }

        private Dictionary<string, decimal> GetProductPriceMap(TrolleyInfo trolleyInfo)
        {
            return trolleyInfo.Products.ToDictionary(q => q.Name, q => q.Price);
        }

        private List<TrolleySpecial> GetApplicableSpecials(List<TrolleySpecial> trolleySpecials, decimal maxPrice, Dictionary<string, int> shoppedProducts)
        {
            var result = trolleySpecials
                            .Where(s => s.Total < maxPrice &&
                                       s.Quantities.All(q => shoppedProducts.ContainsKey(q.Name) && shoppedProducts[q.Name] >= q.Quantity))
                            .ToList();
            return result;
        }
    }
}
