#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using SymuEngine.Results;

#endregion

namespace SymuEngine.Engine
{
    public class SimulationEngines : SimulationEngine
    {
        public List<SimulationEngine> List { get; } = new List<SimulationEngine>();

        public override SimulationResults Process()
        {
            foreach (var simulation in List)
            {
                simulation.SetEnvironment(Environment);
                SimulationResults.AddRange(simulation.Process());
            }

            return SimulationResults;
        }

        public override void InitializeIteration()
        {
            foreach (var simulation in List)
            {
                simulation.InitializeIteration();
            }

            SimulationResults.Clear();
        }

        protected override void PostProcess()
        {
        }

        protected override void AnalyzeIteration()
        {
        }
    }
}