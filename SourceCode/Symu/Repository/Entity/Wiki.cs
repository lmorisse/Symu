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
using Symu.DNA;
using Symu.DNA.OneModeNetworks.Knowledge;
using Symu.Messaging.Templates;

#endregion

namespace Symu.Repository.Entity
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class Wiki : Database
    {
        public static Wiki CreateInstance(IId agentId, OrganizationModels organizationModels, MetaNetwork metaNetwork)
        {
            CommunicationTemplate communication = new ViaPlatformTemplate();
            var entity = new DatabaseEntity(agentId, communication);
            return new Wiki(entity, organizationModels, metaNetwork);
        }

        private Wiki(DatabaseEntity entity, OrganizationModels organizationModels,
            MetaNetwork metaNetwork) : base(entity, organizationModels, metaNetwork)
        {
        }

    }
}