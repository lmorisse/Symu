#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;

#endregion

namespace Symu.Engine
{
    public class SymuEngines : SymuEngine
    {
        public List<SymuEngine> List { get; } = new List<SymuEngine>();

        public override void Process()
        {
            foreach (var simulation in List)
            {
                simulation.SetEnvironment(Environment);
                simulation.Process();
                SimulationResults.AddRange(simulation.SimulationResults);
            }
        }

        public override void InitializeIteration()
        {
            foreach (var simulation in List)
            {
                simulation.InitializeIteration();
            }

            SimulationResults.Clear();
        }
    }
}