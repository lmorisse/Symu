#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Scenario;

#endregion

namespace Symu.Engine
{
    public class Iterations
    {
        public ushort Max { get; set; } = 1;
        public float Step { get; set; } = 0.1F;
        public ushort Number { get; set; }

        public bool Stop()
        {
            if (Number < Max)
            {
                return false;
            }

            SetUp();
            return true;
        }

        public virtual void UpdateIteration(List<SimulationScenario> scenarii)
        {
            if (scenarii is null)
            {
                throw new ArgumentNullException(nameof(scenarii));
            }

            Number++;
        }

        public void SetUp()
        {
            Number = 0;
        }
    }
}