#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Murphies;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Networks;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.DNA.Networks.TwoModesNetworks.Sphere;
using Symu.Messaging.Templates;
using Symu.Repository;
using Symu.Repository.Entity;

#endregion

namespace Symu.Classes.Organization
{
    /// <summary>
    ///     A base class for organizationEntity. You must define your own organizationEntity derived classes.
    /// </summary>
    //TODO should be an abstract class
    public class OrganizationEntity : AgentEntity
    {
        public const byte Class = SymuYellowPages.Organization;

        public OrganizationEntity(string name) : base(new UId(0), Class, name)
        { 
            var interactionSphereModel = new InteractionSphereModel();
            MetaNetwork = new MetaNetwork(interactionSphereModel);
        }

        public MetaNetwork MetaNetwork
        {
            get;
        }
        /// <summary>
        ///     Latest unique index of agents
        /// </summary>
        public ushort EntityIndex { get; set; } = 1;

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
        ///     List of all databases accessible to everyone
        /// </summary>
        //todo _network.Resource
        public IEnumerable<DatabaseEntity> Databases => MetaNetwork.Resource.List.OfType<DatabaseEntity>();//{ get; } = new List<DatabaseEntity>();

        /// <summary>
        ///     List of all knowledge
        /// </summary>
        public List<IKnowledge> Knowledge => MetaNetwork.Knowledge.List;

        /// <summary>
        ///     List of all tasks
        /// </summary>
        public List<ITask> Tasks => MetaNetwork.Task.List;

        /// <summary>
        ///     List of the /Maydays handle in the simulation
        /// </summary>
        public MurphyCollection Murphies { get; } = new MurphyCollection();

        public IId NextEntityId()
        {
            return new UId(EntityIndex++);
        }

        /// <summary>
        ///     Add a database accessible to everyone
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(IResource resource)
        {
            //if (!Databases.Contains(database))
            //{
            //    Databases.Add(database);
            //}
            MetaNetwork.Resource.List.Add(resource);
        }

        /// <summary>
        ///     Clear repository of knowledge
        /// </summary>
        public void ClearKnowledge()
        {
            MetaNetwork.Knowledge.Clear();
        }

        /// <summary>
        ///     Add a knowledge to the repository of knowledge
        /// </summary>
        /// <param name="knowledge"></param>
        public void AddKnowledge(IKnowledge knowledge)
        {
            MetaNetwork.Knowledge.Add(knowledge);
        }

        /// <summary>
        ///     Add all knowledge to the repository of knowledge
        /// </summary>
        /// <param name="knowledge"></param>
        public void AddKnowledge(IEnumerable<IKnowledge> knowledge)
        {
            MetaNetwork.Knowledge.Add(knowledge);
        }

        /// <summary>
        ///     Add a task (activity) to the repository of Tasks
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(ITask task)
        {
            MetaNetwork.Task.Add(task);
        }        
        /// <summary>
        ///     Add a list of tasks (activities) to the repository of Tasks
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTasks(IEnumerable<ITask> tasks)
        {
            MetaNetwork.Task.Add(tasks);
        }

        /// <summary>
        ///     Clear organization for multiple iterations simulation
        /// </summary>
        public void Clear()
        {
            //Databases.Clear();
            MetaNetwork.Clear();
            EntityIndex = 1;
        }
    }
}