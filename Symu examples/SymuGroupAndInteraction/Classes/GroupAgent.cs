#region Licence

// Description: SymuBiz - SymuGroupAndInteraction
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.OrgMod.Edges;

#endregion

namespace SymuGroupAndInteraction.Classes
{
    public sealed class GroupAgent : ReactiveAgent
    {
        public const byte Class = 1;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private GroupAgent(SymuEnvironment environment) : base(
            ClassId, environment)
        {
        }

        public static IClassId ClassId => new ClassId(Class);

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static GroupAgent CreateInstance(SymuEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new GroupAgent(environment);
            agent.Initialize();
            return agent;
        }

        public void AddPerson(PersonAgent newPerson)
        {
            if (newPerson == null)
            {
                throw new ArgumentNullException(nameof(newPerson));
            }

            // this new person is member of the group
            newPerson.GroupId = AgentId;
            ActorOrganization.CreateInstance(Environment.MainOrganization.MetaNetwork.ActorOrganization, newPerson.AgentId, AgentId);
            // All the members of this group have interactions
            var actorIds = Environment.MainOrganization.MetaNetwork.ActorOrganization.SourcesFilteredByTarget(AgentId).ToList();
            actorIds.ForEach(actorId => ActorActor.CreateInstance(Environment.MainOrganization.MetaNetwork.ActorActor, actorId, newPerson.AgentId));
        }
    }
}