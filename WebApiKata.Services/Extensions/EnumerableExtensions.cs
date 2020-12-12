using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiKata.Services.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> items, int count)
        {
            if (count == 1)
            {
                return items.Select(t => new T[] { t });
            }

            return GetPermutations(items, count - 1)
                .SelectMany(t => items.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
