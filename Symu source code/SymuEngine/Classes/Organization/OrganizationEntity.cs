#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using SymuEngine.Classes.Agents;
using SymuEngine.Classes.Agents.Models;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Databases;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Classes.Organization
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
        public AgentTemplates Templates { get; } = new AgentTemplates();

        /// <summary>
        ///     List of all databases accessible to everyone
        /// </summary>
        public DatabaseCollection Databases { get; } = new DatabaseCollection();

        /// <summary>
        ///     List of all knowledges
        /// </summary>
        public List<Knowledge> Knowledges { get; } = new List<Knowledge>();

        public ushort NextEntityIndex()
        {
            return EntityIndex++;
        }

        /// <summary>
        ///     Add a database accessible to everyone
        /// </summary>
        /// <param name="database"></param>
        public void AddDatabase(Database database)
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

        public void Clear()
        {
            Databases.Clear();
            Knowledges.Clear();
        }
    }
}