#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common;
using Symu.Environment;

#endregion

namespace Symu.Classes.Scenario
{
    public class TaskBasedScenario : ScenarioAgent
    {/// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static TaskBasedScenario CreateInstance(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new TaskBasedScenario(environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private TaskBasedScenario(SymuEnvironment environment) : base(null, environment)
        {
            Environment.IterationResult.Tasks.On = true;
        }

        /// <summary>
        ///     Number of steps max to process
        ///     In the same unit of TimeStepType
        ///     Use NoLimit for a scenario with no end
        /// </summary>
        public ushort NumberOfTasks { get; set; }

        public static sbyte NoLimit { get; } = -1;

        public override void PreStep()
        {
            base.PreStep();
            if (NumberOfTasks != NoLimit && Environment.IterationResult.Tasks.Done >= NumberOfTasks - 1)
            {
                State = AgentState.Stopping;
            }
        }

        public override ReactiveAgent Clone()
        {
            var clone = new TaskBasedScenario(Environment)
            {
                NumberOfTasks = NumberOfTasks
            };
            return clone;
        }
    }
}