#region Licence

// Description: SymuBiz - SymuTools
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Common.Math.ProbabilityDistributions;

#endregion

namespace Symu.Common
{
    public static class List
    {
        public static List<TItem> Shuffle<TItem>(this IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var agentIds = items.ToList();
            if (!agentIds.Any())
            {
                return new List<TItem>();
            }

            var list = new List<TItem>();
            list.AddRange(agentIds);
            for (var i = list.Count - 1; i > 1; i--)
            {
                var random = new Random();
                var rnd = random.Next(i + 1);

                var value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }

            return list;
        }

        public static List<TItem> Shuffle<TItem>(this List<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!items.Any())
            {
                return items;
            }

            for (var i = items.Count - 1; i > 1; i--)
            {
                var rnd = DiscreteUniform.Sample(i);
                var value = items[rnd];
                items[rnd] = items[i];
                items[i] = value;
            }

            return items;
        }

        public static bool Equals<TType>(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type == typeof(TType);
        }

        public static byte Average(this List<byte> values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                return 0;
            }

            var sum = values.Aggregate<byte, byte>(0, (current, value) => (byte) (current + value));

            return (byte) System.Math.Round(1.0 * sum / values.Count);
        }

        public static float Average(this ushort[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                return 0;
            }

            var sum = values.Aggregate<ushort, float>(0, (current, value) => current + value);

            return sum / values.Length;
        }
    }
}