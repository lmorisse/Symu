#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent.Models;

#endregion

namespace SymuEngine.Classes.Organization
{
    /// <summary>
    ///     List of the models used by the organizationEntity
    /// </summary>
    public class OrganizationModels
    {
        /// <summary>
        ///     If set, the organizationEntity flexibility performance will be followed and stored during the simulation
        /// </summary>
        public bool FollowGroupFlexibility { get; set; }

        /// <summary>
        ///     If set, the organizationEntity knowledge and belief performance will be followed and stored during the simulation
        /// </summary>
        public bool FollowGroupKnowledge { get; set; }

        /// <summary>
        ///     Agent knowledge learning model
        /// </summary>
        public ModelEntity Learning { get; set; } = new ModelEntity();

        /// <summary>
        ///     Agent knowledge forgetting model
        /// </summary>
        public ModelEntity Forgetting { get; set; } = new ModelEntity();

        /// <summary>
        ///     If true, allow multiple blockers at the same time
        ///     If false, will check new blockers only if there is no blocker
        /// </summary>
        public bool MultipleBlockers { get; set; } = false;

        public void CopyTo(OrganizationModels entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Learning.CopyTo(entity.Learning);
            Forgetting.CopyTo(entity.Forgetting);
            entity.FollowGroupFlexibility = FollowGroupFlexibility;
            entity.FollowGroupKnowledge = FollowGroupKnowledge;
        }
    }
}