#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Murphies;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;

#endregion

namespace Symu.Classes.Organization
{
    /// <summary>
    ///     A base class for organization that encapsulates the metaNetwork, the organizational models and so on.
    ///     You must define your own organization derived classes.
    /// </summary>
    //TODO should be an abstract class
    public class MainOrganization 
    {
        public const byte Class = ClassIdCollection.Organization;
        public static IClassId ClassId => new ClassId(Class);

        public MainOrganization(string name) 
        {
            Name = name;
            MetaNetwork = new GraphMetaNetwork(Models.InteractionSphere);
        }


        public string Name { get; set; }

        public GraphMetaNetwork MetaNetwork { get; protected set; }

        /// <summary>
        ///     List of the models used by the organizationEntity
        ///     You can set your own derived Models
        /// </summary>
        public MainOrganizationModels Models { get; set; } = new MainOrganizationModels();

        /// <summary>
        ///     List of the agent templates that are available
        ///     Use to set attributes of those templates
        ///     and to set agent with templates
        /// </summary>
        public AgentTemplates Templates { get; set; } = new AgentTemplates();

        /// <summary>
        ///     List of the communication templates that are available
        ///     Use to set attributes of those templates
        /// </summary>
        public CommunicationTemplates Communication { get; set; } = new CommunicationTemplates();

        /// <summary>
        ///     List of the /Maydays handle in the simulation
        /// </summary>
        public MurphyCollection Murphies { get; protected set; } = new MurphyCollection();

        public virtual MainOrganization Clone()
        {
            var clone = new MainOrganization(Name);
            CopyTo(clone);
            return clone;
        }

        public void CopyTo(MainOrganization entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.MetaNetwork = MetaNetwork.Clone();
            entity.Models = Models.Clone();
            entity.Murphies = Murphies.Clone();
            entity.Communication = Communication;
            entity.Templates = Templates;
        }
    }
}