#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Murphies;
using Symu.Common.Interfaces;

using Symu.DNA.Entities;
using Symu.DNA.GraphNetworks;
using Symu.DNA.GraphNetworks.TwoModesNetworks.Sphere;
using Symu.Messaging.Templates;
using Symu.Repository;
using Symu.Repository.Edges;
using Symu.Repository.Entities;

#endregion

namespace Symu.Classes.Organization
{
    /// <summary>
    ///     A base class for organization that encapsulates the metaNetwork, the organizational models and so on.
    /// You must define your own organization derived classes.
    /// </summary>
    //TODO should be an abstract class
    public class Organization //: Entity
    {
        public const byte Class = ClassIdCollection.Organization;
        public static IClassId ClassId => new ClassId(Class);

        public Organization(string name) //: base(new MetaNetwork(), Class, name)
        {
            Name = name;
            MetaNetwork = new MetaNetwork(Models.InteractionSphere);
        }

        public string Name { get; set; }

        public MetaNetwork MetaNetwork { get; protected set; }

        /// <summary>
        ///     List of the models used by the organizationEntity
        ///     You can set your own derived Models
        /// </summary>
        public OrganizationModels Models { get; set; } = new OrganizationModels();

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
        ///     List of all databases of the simulation
        /// </summary>
        public IEnumerable<IAgentId> DatabaseIds => MetaNetwork.Resource.GetEntityIds<Database>();

        /// <summary>
        ///     List of the /Maydays handle in the simulation
        /// </summary>
        public MurphyCollection Murphies { get; protected set; } = new MurphyCollection();

        public virtual Organization Clone()
        {
            var clone = new Organization(Name);
            CopyTo(clone);
            return clone;
        }

        public void CopyTo(Organization entity)
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