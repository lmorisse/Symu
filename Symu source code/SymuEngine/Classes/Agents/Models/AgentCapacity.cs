#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using static SymuTools.Constants;

#endregion

namespace SymuEngine.Classes.Agents.Models
{
    public class AgentCapacity
    {
        /// <summary>
        ///     Initial capacity
        /// </summary>
        public float Initial { get; set; }

        public float Actual { get; private set; }
        public bool HasCapacity => Actual > Tolerance;

        /// <summary>
        ///     Reset RemainingCapacity to InitialCapacity
        /// </summary>
        public void Reset()
        {
            Set(Initial);
        }

        /// <summary>
        ///     Decrement the remaining capacity
        /// </summary>
        /// <param name="decrement"></param>
        public void Decrement(float decrement)
        {
            Actual -= decrement;
            if (Actual < 0)
            {
                Actual = 0;
            }
        }

        /// <summary>
        ///     Multiply the remaining capacity
        /// </summary>
        /// <param name="value"></param>
        public void Multiply(float value)
        {
            Actual *= value;
        }

        /// <summary>
        ///     Set the remaining capacity
        /// </summary>
        /// <param name="value"></param>
        public void Set(float value)
        {
            Actual = value;
            if (Actual < 0)
            {
                Actual = 0;
            }
        }
    }
}