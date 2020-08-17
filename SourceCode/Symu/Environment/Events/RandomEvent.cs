#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Math.ProbabilityDistributions;

#endregion

namespace Symu.Environment.Events
{
    /// <summary>
    ///     SymuEvent helps you schedule random events that happen during the simulation
    /// </summary>
    public class RandomEvent : SymuEvent
    {
        private float _ratio;

        public float Ratio
        {
            get => _ratio;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("Ratio should be between 0 and 1");
                }

                _ratio = value;
            }
        }

        public override bool Trigger(ushort step)
        {
            return Bernoulli.Sample(_ratio);
        }
    }
}