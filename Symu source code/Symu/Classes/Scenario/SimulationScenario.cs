#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates;
using Symu.Environment;

#endregion

namespace Symu.Classes.Scenario
{
    public class SimulationScenario : Agent
    {
        public SimulationScenario(object parent, SymuEnvironment environment) : base(
            new AgentId(environment.Organization.NextEntityIndex(), ScenarioEntity.ClassKey), environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            Entity = new ScenarioEntity(Id.Key);
            Parent = parent;
            SetCognitive(new StandardAgentTemplate());
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