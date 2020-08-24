#region Licence

// Description: SymuBiz - SymuMurphiesAndBlockers
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
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;

        public PersonAgent(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(id, Class), environment, template)
        {
        }

        public AgentId GroupId { get; set; }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        public IEnumerable<Knowledge> Knowledges => Environment.Organization.Knowledges;
        public InternetAccessAgent Internet => ((ExampleEnvironment) Environment).Internet;

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
            Cognitive.TasksAndPerformance.CanPerformTaskOnWeekEnds = true;
            Cognitive.TasksAndPerformance.TasksLimit.LimitSimultaneousTasks = true;
            Cognitive.TasksAndPerformance.TasksLimit.MaximumSimultaneousTasks = 1;
            Cognitive.InteractionPatterns.IsolationCyclicity = Cyclicity.None;
            Cognitive.InteractionPatterns.AgentCanBeIsolated = Frequency.Never;
            Cognitive.InteractionPatterns.AllowNewInteractions = false;
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
                KnowledgeModel.AddKnowledge(knowledge.Id, ((ExampleEnvironment) Environment).KnowledgeLevel,
                    Cognitive.InternalCharacteristics);
                //Beliefs are added with knowledge based on DefaultBeliefLevel of the worker cognitive template
                BeliefsModel.AddBelief(knowledge.Id, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1,
                // Creator is randomly  a person of the group - for the incomplete information murphy
                Creator = Environment.WhitePages.FilteredAgentIdsByClassId(Class).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }

        public override void TryRecoverBlockerIncompleteKnowledgeExternally(SymuTask task, Blocker blocker,
            ushort knowledgeId,
            byte knowledgeBit)
        {
            if (blocker == null)
            {
                throw new ArgumentNullException(nameof(blocker));
            }

            var attachments = new MessageAttachments();
            attachments.Add(blocker);
            attachments.Add(task);
            attachments.KnowledgeId = knowledgeId;
            attachments.KnowledgeBit = knowledgeBit;
            Send(Internet.AgentId, MessageAction.Ask, SymuYellowPages.Help, attachments,
                CommunicationMediums.ViaAPlatform);
        }
    }
}