#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace Symu.Results
{
    /// <summary>
    ///     Structure to store result with a sum, a mean, a standard deviation
    /// </summary>
    public class StatisticalResultStruct
    {
        public StatisticalResultStruct(float sum, float mean, float standardDeviation, ushort step)
        {
            Sum = sum;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Step = step;
        }

        /// <summary>
        ///     Global knowledge or belief in the organization
        /// </summary>
        public float Sum { get; }

        /// <summary>
        ///     Mean Knowledge or belief per agent in the organization
        /// </summary>
        public float Mean { get; }

        /// <summary>
        ///     Standard deviation of knowledge or belief
        /// </summary>
        public float StandardDeviation { get; }

        public ushort Step { get; }

        public override string ToString()
        {
            return "Sum " + Sum + "Average " + Mean + " - stdDev " + StandardDeviation + " / step" + Step;
        }

        public static StatisticalResultStruct SetStruct(ushort step,
            IReadOnlyList<byte> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var floats = values.Select(Convert.ToSingle).ToList();
            return SetStruct(step, floats);
        }

        public static StatisticalResultStruct SetStruct(ushort step,
            IReadOnlyList<float> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            float sum;
            float mean;
            float stdDev;
            switch (values.Count)
            {
                case 0:
                    sum = 0;
                    mean = 0;
                    stdDev = 0;
                    break;
                case 1:
                    sum = values[0];
                    mean = values[0];
                    stdDev = 0;
                    break;
                default:
                    sum = values.Sum();
                    mean = values.Average();
                    stdDev = (float)values.StandardDeviation();
                    break;
            }

            return new StatisticalResultStruct(sum, mean, stdDev, step);
        }
    }
}