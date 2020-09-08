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
using Symu.DNA.Networks;
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
        public static Wiki CreateInstance(IId id, OrganizationModels organizationModels, MetaNetwork metaNetwork)
        {
            return new Wiki(id, new ViaPlatformTemplate(), organizationModels, metaNetwork);
        }

        private Wiki(IId id, CommunicationTemplate communication, OrganizationModels organizationModels,
            MetaNetwork metaNetwork) : base(id, communication, organizationModels, metaNetwork)
        {
        }

    }
}