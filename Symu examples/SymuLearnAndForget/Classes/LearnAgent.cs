#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Task;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository.Entities;

#endregion

namespace SymuLearnAndForget.Classes
{
    public class LearnAgent : CognitiveAgent
    {
        public const byte Class = 2;
        public static IClassId ClassId => new ClassId(Class);
        protected ExampleOrganization Organization => ((ExampleEnvironment) Environment).ExampleOrganization;
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static LearnAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new LearnAgent(environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        protected LearnAgent(SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            ClassId, environment, template)
        {
            Wiki = Organization.WikiEntity;
            Knowledge = GetKnowledge();
        }

        protected Database Wiki { get; }
        protected Knowledge Knowledge { get; set; }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums = CommunicationMediums.ViaAPlatform;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            KnowledgeModel.AddKnowledge(Organization.Knowledge.EntityId,
                Organization.KnowledgeLevel, Cognitive.InternalCharacteristics);
        }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(0)
            {
                Parent = Schedule.Step,
                Weight = 1
            };
            Post(task);
        }

        /// <summary>
        ///     each task allows to learn a little more about knowledge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
        }

        private Knowledge GetKnowledge()
        {
            var knowledgeId = Environment.Organization.MetaNetwork.Knowledge.List.First().EntityId;
            return (Knowledge)Environment.Organization.MetaNetwork.Knowledge.GetEntity(knowledgeId);
        }
    }
}