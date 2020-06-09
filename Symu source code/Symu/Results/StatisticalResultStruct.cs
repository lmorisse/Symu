#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

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
    }
}