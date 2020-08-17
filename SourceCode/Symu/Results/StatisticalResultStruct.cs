#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using static Symu.Tools.Constants;

#endregion

namespace Symu.Results
{
    /// <summary>
    ///     Structure to store result with a sum, a mean, a standard deviation
    /// </summary>
    public class StatisticalResultStruct
    {
        /// <summary>
        ///     Maximum potential value of the Sum : the sum of potential of each agent, not the potential of the mean
        /// </summary>
        private readonly float _potential;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="potential">Maximum potential value of the Sum : the sum of potential of each agent</param>
        /// <param name="step"></param>
        public StatisticalResultStruct(float sum, float mean, float standardDeviation, float min, float max,
            float potential, ushort step)
        {
            Sum = sum;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Minimum = min;
            Maximum = max;
            _potential = potential;
            Step = step;
        }

        /// <summary>
        ///     Sum in the organization
        /// </summary>
        public float Sum { get; }

        /// <summary>
        ///     Mean per agent in the organization
        /// </summary>
        public float Mean { get; }

        /// <summary>
        ///     Standard deviation
        /// </summary>
        public float StandardDeviation { get; }

        /// <summary>
        ///     Minimum in the organization
        /// </summary>
        public float Minimum { get; }

        /// <summary>
        ///     Maximum in the organization
        /// </summary>
        public float Maximum { get; }

        /// <summary>
        ///     The percentage is calculated based on the Sum and the Maximum potential value
        /// </summary>
        public float Percentage => Math.Abs(_potential) < Tolerance ? 0 : 100F * Sum / _potential;

        /// <summary>
        ///     The standard deviation normalized for the percentage
        /// </summary>
        public float StdDevPercentage => Math.Abs(_potential) < Tolerance ? 0 : 100F * StandardDeviation / _potential;

        public ushort Step { get; }

        public override string ToString()
        {
            return "Sum " + Sum + "Average " + Mean + " - stdDev " + StandardDeviation + " / step" + Step;
        }

        /// <summary>
        ///     Factory method for the result
        /// </summary>
        /// <param name="step"></param>
        /// <param name="values"></param>
        /// <param name="potential">Maximum potential value of the Sum : the sum of potential of each agent</param>
        /// <returns></returns>
        public static StatisticalResultStruct SetStruct(ushort step,
            IReadOnlyList<byte> values, float potential)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var floats = values.Select(Convert.ToSingle).ToList();
            return SetStruct(step, floats, potential);
        }

        /// <summary>
        ///     Factory method for the struct
        /// </summary>
        /// <param name="step"></param>
        /// <param name="values"></param>
        /// <param name="potential"></param>
        /// <returns></returns>
        public static StatisticalResultStruct SetStruct(ushort step,
            IReadOnlyList<float> values, float potential)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            float sum;
            float mean;
            float min;
            float max;
            float stdDev;
            switch (values.Count)
            {
                case 0:
                    sum = 0;
                    mean = 0;
                    min = 0;
                    max = 0;
                    stdDev = 0;
                    break;
                case 1:
                    sum = values[0];
                    mean = values[0];
                    min = values[0];
                    max = values[0];
                    stdDev = 0;
                    break;
                default:
                    sum = values.Sum();
                    mean = values.Average();
                    min = values.Minimum();
                    max = values.Maximum();
                    stdDev = (float) values.StandardDeviation();
                    break;
            }

            return new StatisticalResultStruct(sum, mean, stdDev, min, max, potential, step);
        }
    }
}