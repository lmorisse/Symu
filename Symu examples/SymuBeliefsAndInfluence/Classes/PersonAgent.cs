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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.Networks.OneModeNetworks;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Entity;

#endregion

namespace SymuBeliefsAndInfluence.Classes
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(IId id, SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            var agent = new PersonAgent(id, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(IId id, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(id, Class), environment, template)
        {
        }

        public IEnumerable<IKnowledge> Knowledges => Environment.Organization.Knowledge;
        public IEnumerable<IAgentId> Influencers => ((ExampleEnvironment) Environment).Influencers.Select(x => x.AgentId);

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
            foreach (var knowledge in Knowledges)
            {
                KnowledgeModel.AddKnowledge(knowledge.Id, KnowledgeLevel.FullKnowledge,
                    Cognitive.InternalCharacteristics);
                //Beliefs are added with knowledge based on DefaultBeliefLevel of the worker cognitive template
                BeliefsModel.AddBelief(knowledge.Id, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1
            };
            task.SetKnowledgesBits(Environment.Organization.Murphies.IncompleteBelief, Knowledges, 1);
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
                KnowledgeId = (IId) blocker.Parameter,
                KnowledgeBit = (byte) blocker.Parameter2
            };
            SendToMany(Influencers, MessageAction.Ask, SymuYellowPages.Belief, attachments, CommunicationMediums.Email);
        }
    }
}