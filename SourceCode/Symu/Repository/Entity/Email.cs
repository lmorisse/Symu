#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Knowledges;
using Symu.Messaging.Templates;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class Email : Database
    {
        public static Email CreateInstance(UId id, OrganizationModels organizationModels, KnowledgeNetwork knowledgeNetwork)
        {
            CommunicationTemplate communication = new EmailTemplate();
            var entity = new DatabaseEntity(id, communication);
            return new Email(entity, organizationModels, knowledgeNetwork);
        }

        private Email(DatabaseEntity entity, OrganizationModels organizationModels,
            KnowledgeNetwork knowledgeNetwork) : base(entity, organizationModels, knowledgeNetwork)
        {
        }

    }
}