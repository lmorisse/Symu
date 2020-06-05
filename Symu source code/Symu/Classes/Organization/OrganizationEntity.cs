#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models;
using Symu.Classes.Murphies;
using Symu.Repository;
using Symu.Repository.Networks.Databases;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Classes.Organization
{
    /// <summary>
    ///     A base class for organizationEntity. You must define your own organizationEntity derived classes.
    /// </summary>
    //TODO should be an abstract class
    public class OrganizationEntity : AgentEntity
    {
        public const byte ClassKey = SymuYellowPages.Organization;

        public OrganizationEntity(string name) : base(0, ClassKey, name)
        {
        }

        /// <summary>
        ///     Latest unique index of agents
        /// </summary>
        public ushort EntityIndex { get; set; } = 1;

        /// <summary>
        ///     List of the models used by the organizationEntity
        ///     You can set your own derived Models
        /// </summary>
        public OrganizationModels Models { get; protected set; } = new OrganizationModels();

        /// <summary>
        ///     List of the agent templates that are available
        ///     Use to set attributes of those templates
        ///     and to set agent with templates
        /// </summary>
        public AgentTemplates Templates { get; set; } = new AgentTemplates();

        /// <summary>
        ///     List of all databases accessible to everyone
        /// </summary>
        public List<DataBaseEntity> Databases { get; } = new List<DataBaseEntity>();

        /// <summary>
        ///     List of all knowledges
        /// </summary>
        public List<Knowledge> Knowledges { get; } = new List<Knowledge>();

        /// <summary>
        ///     List of the /Maydays handle in the symu
        /// </summary>
        public MurphyCollection Murphies { get; } = new MurphyCollection();

        public ushort NextEntityIndex()
        {
            return EntityIndex++;
        }

        /// <summary>
        ///     Add a database accessible to everyone
        /// </summary>
        /// <param name="database"></param>
        public void AddDatabase(DataBaseEntity database)
        {
            if (!Databases.Contains(database))
            {
                Databases.Add(database);
            }
        }

        /// <summary>
        ///     Add a knowledge to the repository of knowledges
        /// </summary>
        /// <param name="knowledge"></param>
        public void AddKnowledge(Knowledge knowledge)
        {
            if (!Knowledges.Contains(knowledge))
            {
                Knowledges.Add(knowledge);
            }
        }

        /// <summary>
        ///     Clear organization for multiple iterations simulation
        /// </summary>
        public void Clear()
        {
            Databases.Clear();
            Knowledges.Clear();
            EntityIndex = 1;
        }
    }
}