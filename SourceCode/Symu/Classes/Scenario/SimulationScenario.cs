#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Common.Interfaces.Agent;
using Symu.Environment;

#endregion

namespace Symu.Classes.Scenario
{
    public class SimulationScenario : ReactiveAgent
    {
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static SimulationScenario CreateInstance(object parent, SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new SimulationScenario(parent, environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        protected SimulationScenario(object parent, SymuEnvironment environment) : base(
            new AgentId(environment.Organization.NextEntityId(), ScenarioEntity.Class), environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            Entity = new ScenarioEntity(AgentId.Id);
            Parent = parent;
        }

        public bool Success { get; set; }
        public ScenarioEntity Entity { get; }
        public ushort Day0 { get; set; } = 0;
        protected object Parent { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual SimulationScenario Clone()
        {
            var clone = new SimulationScenario(Parent, Environment);
            return clone;
        }

        public virtual void SetUp()
        {
        }
    }
}