#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Environment;

#endregion

namespace SymuEngine.Classes.Scenario
{
    public class TimeStepScenario : SimulationScenario
    {
        public TimeStepScenario(SymuEnvironment environment) : base(null, environment)
        {
        }

        /// <summary>
        ///     Number of steps max to process
        ///     In the same unit of TimeStepType
        ///     Use NoLimit for a scenario with no end
        /// </summary>
        public ushort NumberOfSteps { get; set; }

        public static sbyte NoLimit { get; } = -1;

        public override void PreStep()
        {
            base.PreStep();
            if (NumberOfSteps != NoLimit && TimeStep.Step == NumberOfSteps - 1)
            {
                State = AgentState.Stopping;
            }
        }

        public override SimulationScenario Clone()
        {
            var clone = new TimeStepScenario(Environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            return clone;
        }
    }
}