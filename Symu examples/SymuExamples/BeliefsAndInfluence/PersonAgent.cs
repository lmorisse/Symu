#region Licence

// Description: SymuBiz - SymuBeliefsAndInfluence
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
using Symu.Classes.Blockers;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.OrgMod.Entities;
using Symu.Repository;
using Symu.Repository.Entities;

#endregion

namespace SymuExamples.BeliefsAndInfluence
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;

        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(IAgentId entityId, SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(
            entityId, environment, template)
        {
        }

        public static IClassId ClassId => new ClassId(Class);
        private ExampleMainOrganization MainOrganization => ((ExampleEnvironment) Environment).ExampleMainOrganization;

        public IEnumerable<IAgentId> Influencers => MainOrganization.Influencers.Select(x => x.AgentId);

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var entity = new ActorEntity(environment.MainOrganization.ArtifactNetwork);
            var agent = new PersonAgent(entity.EntityId, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     Customize the cognitive architecture of the agent
        ///     After setting the Agent template
        /// </summary>
        protected override void SetCognitive()
        {
            base.SetCognitive();

            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.InteractionPatterns.AllowNewInteractions = false;
            Cognitive.InteractionCharacteristics.PreferredCommunicationMediums =
                CommunicationMediums.Email;
            Cognitive.InternalCharacteristics.InfluentialnessRateMin = 0;
            Cognitive.InternalCharacteristics.InfluentialnessRateMax = 0F;
            Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel = BeliefLevel.NeitherAgreeNorDisagree;
        }

        /// <summary>
        ///     Customize the models of the agent
        ///     After setting the Agent basics models
        /// </summary>
        public override void SetModels()
        {
            base.SetModels();
            foreach (var knowledgeId in Environment.MainOrganization.ArtifactNetwork.Knowledge.GetEntityIds())
            {
                KnowledgeModel.AddKnowledge(knowledgeId, KnowledgeLevel.FullKnowledge,
                    Cognitive.InternalCharacteristics);
                BeliefsModel.AddBeliefFromKnowledgeId(knowledgeId, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1
            };
            task.SetKnowledgesBits(Environment.MainOrganization.Murphies.IncompleteBelief,
                Environment.MainOrganization.ArtifactNetwork.Knowledge.GetEntities<IKnowledge>(), 1);
            Post(task);
        }

        /// <summary>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="blocker"></param>
        /// <returns></returns>
        public override void TryRecoverBlockerIncompleteBelief(SymuTask task, Blocker blocker)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (blocker is null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            // RiskAversionThreshold has been exceeded =>
            // Worker don't want to do the task, the task is blocked in base method
            // Ask advice from influencers
            var attachments = new MessageAttachments
            {
                KnowledgeId = (IAgentId) blocker.Parameter,
                KnowledgeBit = (byte) blocker.Parameter2
            };
            SendToMany(Influencers, MessageAction.Ask, SymuYellowPages.Belief, attachments, CommunicationMediums.Email);
        }
    }
}