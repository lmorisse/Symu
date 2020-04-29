#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using SymuEngine.Classes.Agent;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Databases.Repository;
using SymuEngine.Repository.Networks.Knowledge.Repository;

#endregion

namespace SymuEngine.Classes.Organization
{
    /// <summary>
    ///     A base class for organizationEntity. You must define your own organizationEntity derived classes.
    /// </summary>
    //TODO should be an abstract class
    public class OrganizationEntity : AgentEntity
    {
        public const byte classKey = SymuYellowPages.organization;

        public OrganizationEntity(string name) : base(0, classKey, name)
        {
        }

        /// <summary>
        ///     Latest unique index of agents
        /// </summary>
        public ushort EntityIndex { get; set; } = 1;

        /// <summary>
        ///     List of the models used by the organizationEntity
        ///     You can set your own derived OrganizationModels
        /// </summary>
        public OrganizationModels OrganizationModels { get; protected set; } = new OrganizationModels();

        /// <summary>
        ///     List of all databases accessible to everyone
        /// </summary>
        public Databases Databases { get; } = new Databases();

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
    }
}