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
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace Symu.Classes.Scenario
{
    public class ScenarioAgent : ReactiveAgent
    {

        public const byte Class = SymuYellowPages.Scenario;
        public static IClassId ClassId => new ClassId(Class);
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static ScenarioAgent CreateInstance(object parent, SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new ScenarioAgent(parent, environment);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        protected ScenarioAgent(object parent, SymuEnvironment environment) : base(
            environment?.WhitePages.NextAgentId(Class), environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            Parent = parent;
        }

        public bool Success { get; set; }
        //public ScenarioEntity Entity { get; }
        public ushort Day0 { get; set; } 
        protected object Parent { get; set; }
        public bool IsActive { get; set; } = true;

        public override ReactiveAgent Clone()
        {
            return new ScenarioAgent(Parent, Environment);
        }

        public virtual void SetUp()
        {
        }
    }
}