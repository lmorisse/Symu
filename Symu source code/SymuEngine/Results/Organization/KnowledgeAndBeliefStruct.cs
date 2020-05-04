#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Results.Organization
{
    /// <summary>
    ///     Structure to store group knowledge information
    /// </summary>
    public class KnowledgeAndBeliefStruct
    {
        public KnowledgeAndBeliefStruct(float sum, float mean, float standardDeviation, float learning,
            float forgetting, float obsolescence, ushort step)
        {
            Sum = sum;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Learning = learning;
            Forgetting = forgetting;
            Obsolescence = obsolescence;
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

        /// <summary>
        ///     Total learning during the simulation
        /// </summary>
        public float Learning { get; }

        /// <summary>
        ///     Total forgetting during the simulation
        /// </summary>
        public float Forgetting { get; }

        /// <summary>
        ///     Global Knowledge or belief obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public float Obsolescence { get; }

        public ushort Step { get; }

        public override string ToString()
        {
            return "Sum " + Sum + "Average " + Mean + " - stdDev " + StandardDeviation + " - learning " + Learning +
                   " - Forgetting " + Forgetting + " - obsolescence " + Obsolescence + " / step" + Step;
        }
    }
}