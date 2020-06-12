#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
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
using Symu.Classes.Task;
using Symu.Common;
using Symu.Environment;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;
using Symu.Tools;
using Symu.Tools.Math.ProbabilityDistributions;

#endregion

namespace SymuScenariosAndEvents.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public PersonAgent(ushort agentKey, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(agentKey, ClassKey), environment, template)
        {
        }

        public AgentId GroupId { get; set; }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        public List<Knowledge> Knowledges => Environment.Organization.Knowledges;

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
            foreach (var knowledge in Knowledges)
            {
                KnowledgeModel.AddKnowledge(knowledge.Id, KnowledgeLevel.Intermediate,
                    Cognitive.InternalCharacteristics);
                //Beliefs are added with knowledge based on DefaultBeliefLevel of the worker cognitive template
                BeliefsModel.AddBelief(knowledge.Id, Cognitive.KnowledgeAndBeliefs.DefaultBeliefLevel);
            }
        }

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                // Weight is randomly distributed around 1, but has a minimum of 0
                Weight = Math.Max(0, Normal.Sample(1, 0.1F * Environment.Organization.Models.RandomLevelValue)),
                // Creator is randomly  a person of the group - for the incomplete information murphy
                Creator = Environment.WhitePages.FilteredAgentIdsByClassKey(ClassKey).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }
    }
}