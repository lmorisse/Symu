#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
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
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA.Entities;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Entities;

#endregion

namespace SymuScenariosAndEvents.Classes
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;
        public static ClassId ClassId => new ClassId(Class);
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new PersonAgent(environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            ClassId, environment, template)
        {
        }

        public IAgentId GroupId { get; set; }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();
            Cognitive.KnowledgeAndBeliefs.HasKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialKnowledge = true;
            Cognitive.KnowledgeAndBeliefs.HasBelief = true;
            Cognitive.KnowledgeAndBeliefs.HasInitialBelief = true;
            Cognitive.TasksAndPerformance.CanPerformTask = true;
            Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = false;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 1;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.InteractionPatterns.AllowNewInteractions = true;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            foreach (var knowledgeId in Environment.Organization.MetaNetwork.Knowledge.GetEntityIds())
            {
                KnowledgeModel.AddKnowledge(knowledgeId, KnowledgeLevel.Intermediate,
                    Cognitive.InternalCharacteristics);
                BeliefsModel.AddBeliefFromKnowledgeId(knowledgeId, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                // Weight is randomly distributed around 1, but has a minimum of 0
                Weight = Math.Max(0, Normal.Sample(1, 0.1F * Environment.RandomLevelValue)),
                // Creator is randomly  a person of the group - for the incomplete information murphy
                Creator = (AgentId)Environment.WhitePages.FilteredAgentIdsByClassId(ClassId).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Environment.Organization.MetaNetwork.Knowledge.GetEntities<IKnowledge>(), 1);
            Post(task);
        }

        public override void OnAfterTaskProcessorStart()
        {
            base.OnAfterTaskProcessorStart();
            TaskProcessor.OnAfterSetTaskDone += AfterSetTaskDone;
        }

        private void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            if (!(e.Task.Parent is Message))
            {
                // warns the group that he has performed the task
                Send(GroupId, MessageAction.Close, SymuYellowPages.Task, CommunicationMediums.Email);
            }
        }
    }
}