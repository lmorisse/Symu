#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace Symu.Classes.Scenario
{
    public class ScenarioAgent : ReactiveAgent
    {
        public const byte Class = SymuYellowPages.Scenario;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        protected ScenarioAgent(object parent, SymuEnvironment environment) : base(
            environment?.AgentNetwork.NextAgentId(Class), environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            Parent = parent;
        }

        public static IClassId ClassId => new ClassId(Class);

        public bool Success { get; set; }

        //public ScenarioEntity Entity { get; }
        public ushort Day0 { get; set; }
        protected object Parent { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
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

        public override ReactiveAgent Clone()
        {
            return new ScenarioAgent(Parent, Environment);
        }

        public virtual void SetUp()
        {
        }
    }
}