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
    public class Email : Database
    {
        public static Email CreateInstance(AgentId agentId, OrganizationModels organizationModels, NetworkKnowledges networkKnowledges)
        {
            CommunicationTemplate communication = new EmailTemplate();
            var entity = new DatabaseEntity(agentId, communication);
            return new Email(entity, organizationModels, networkKnowledges);
        }

        private Email(DatabaseEntity entity, OrganizationModels organizationModels,
            NetworkKnowledges networkKnowledges) : base(entity, organizationModels, networkKnowledges)
        {
        }

    }
}