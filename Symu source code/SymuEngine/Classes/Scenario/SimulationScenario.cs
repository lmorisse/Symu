#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Agent.Models.Templates;
using SymuEngine.Environment;

#endregion

namespace SymuEngine.Classes.Scenario
{
    public class SimulationScenario : Agent.Agent
    {
        public SimulationScenario(ushort key, object parent, SymuEnvironment environment) : base(
            new AgentId(key, ScenarioEntity.classKey), environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            Entity = new ScenarioEntity(key);
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
            var clone = new SimulationScenario(Id.Key, Parent, Environment);
            return clone;
        }

        public virtual void SetUp()
        {
        }
    }
}