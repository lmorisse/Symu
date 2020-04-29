#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Common;
using SymuEngine.Environment;
using SymuEngine.Environment.TimeStep;

#endregion

namespace SymuEngine.Classes.Scenario
{
    public class TimeStepScenario : SimulationScenario
    {
        public TimeStepScenario(ushort key, SymuEnvironment environment) : base(key, null, environment)
        {
        }

        /// <summary>
        ///     Number of steps max to process
        ///     In the same unit of TimeStepType
        ///     Use NoLimit for a scenario with no end
        /// </summary>
        public ushort NumberOfSteps { get; set; }

        public static sbyte NoLimit { get; } = -1;
        public TimeStepType TimeStepType { get; set; }

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
            var clone = new TimeStepScenario(Id.Key, Environment)
            {
                NumberOfSteps = NumberOfSteps,
                TimeStepType = TimeStepType
            };
            return clone;
        }
    }
}