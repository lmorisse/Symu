﻿#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Environment;

#endregion

namespace Symu.Classes.Scenario
{
    public class SimulationScenario : ReactiveAgent
    {
        public SimulationScenario(object parent, SymuEnvironment environment) : base(
            new AgentId(environment.Organization.NextEntityIndex(), ScenarioEntity.Class), environment)
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