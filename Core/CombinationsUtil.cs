using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class CombinationsUtil
    {
        public static IEnumerable<int[]> GetCombinations(int n, int k)
        {
            foreach (var values in GetCombinations(new int[k], k, 0, n))
            {
                yield return values;
            }
        }

        public static IEnumerable<int[]> GetCombinations(int[] result, int k, int startIndex = 0, int n = int.MaxValue)
        {
            if (n - startIndex >= k && k > 0)
            {
                for (int value = startIndex; value < n; value++)
                {
                    if (n - value >= k && k > 0)
                    {
                        result[result.Length - k] = value;
                        if (k == 1 || value == n - 1)
                        {
                            yield return result;
                        }
                        else
                        {
                            {
                                foreach (var values in GetCombinations(result, k - 1, value + 1, n))
                                {
                                    yield return values;
                                }
                            }
                        }
                    }
                }
            }
            else
            {

            }
        }
    }
}
