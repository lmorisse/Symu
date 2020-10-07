#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
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
    public class WikiEntity : Database
    {
        public const byte Class = SymuYellowPages.Wiki;
        public static IClassId ClassId => new ClassId(Class);
        public static WikiEntity CreateInstance(GraphMetaNetwork metaNetwork, OrganizationModels models)
        {
            return new WikiEntity(metaNetwork, models);
        }

        private WikiEntity(){}
        private WikiEntity(GraphMetaNetwork metaNetwork, OrganizationModels models) : base(metaNetwork, models, new ViaPlatformTemplate(), Class)
        {
        }        /// <summary>Creates a new object that is a copy of the current instance, with the same EntityId.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            var clone = new WikiEntity();
            CopyEntityTo(clone);
            return clone;
        }
    }
}