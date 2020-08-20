#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class Wiki : Database
    {
        public static Wiki CreateInstance(AgentId agentId, OrganizationModels organizationModels, NetworkKnowledges networkKnowledges)
        {
            CommunicationTemplate communication = new ViaPlatformTemplate();
            var entity = new DatabaseEntity(agentId, communication);
            return new Wiki(entity, organizationModels, networkKnowledges);
        }

        private Wiki(DatabaseEntity entity, OrganizationModels organizationModels,
            NetworkKnowledges networkKnowledges) : base(entity, organizationModels, networkKnowledges)
        {
        }

    }
}