#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Murphies;
using Symu.Common.Interfaces;
using Symu.Messaging.Templates;
using Symu.OrgMod.Entities;
using Symu.OrgMod.GraphNetworks;
using Symu.SysDyn.Engine;
using Symu.SysDyn.Models;
using Symu.SysDyn.Models.Symu;
using Symu.SysDyn.Models.XMile;

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
            ArtifactNetwork = new GraphMetaNetwork(Models.InteractionSphere);
        }

        public string Name { get; set; }

        /// <summary>
        /// Network of artifacts coming from Symu.OrgMod:
        /// Actor, Role, Resource, Knowledge, Belief, Organization, Task, Event
        /// </summary>
        public GraphMetaNetwork ArtifactNetwork { get; protected set; }
        /// <summary>
        /// Network of models coming from Symu.SysDyn
        /// </summary>
        public ModelNetwork ModelNetwork { get; set; } = new ModelNetwork();

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
        ///     List of the Murphies handle in the simulation
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

            entity.ArtifactNetwork = ArtifactNetwork.Clone();
            entity.ModelNetwork = ModelNetwork.Clone();
            entity.Models = Models.Clone();
            entity.Murphies = Murphies.Clone();
            entity.Communication = Communication;
            entity.Templates = Templates;
        }
    }
}