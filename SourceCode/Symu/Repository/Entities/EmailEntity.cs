#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Symu.Classes.Organization;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace Symu.Repository.Entities
{
    /// <summary>
    ///     Database used to store and search information
    ///     A database is a system where agent store temporary or permanent information
    /// </summary>
    public class EmailEntity : Database
    {
        public new const byte Class = SymuYellowPages.Email;
        public new static IClassId ClassId => new ClassId(Class);

        private EmailEntity()
        {
        }

        private EmailEntity(GraphMetaNetwork metaNetwork, MainOrganizationModels models) : base(metaNetwork, models,
            new EmailTemplate(), Class)
        {
        }


        public static EmailEntity CreateInstance(GraphMetaNetwork metaNetwork, MainOrganizationModels models)
        {
            return new EmailEntity(metaNetwork, models);
        }

        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new EmailEntity();
            CopyEntityTo(clone);
            return clone;
        }
    }
}