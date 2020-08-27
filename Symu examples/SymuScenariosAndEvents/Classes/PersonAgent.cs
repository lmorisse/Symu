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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Environment;
using Symu.Repository;
using Symu.Repository.Entity;

#endregion

namespace SymuScenariosAndEvents.Classes
{
    public sealed class PersonAgent : CognitiveAgent
    {
        public const byte Class = SymuYellowPages.Actor;
        /// <summary>
        /// Factory method to create an agent
        /// Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public static PersonAgent CreateInstance(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template)
        {
            var agent = new PersonAgent(id, environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        /// Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private PersonAgent(UId id, SymuEnvironment environment, CognitiveArchitectureTemplate template) : base(
            new AgentId(id, Class), environment, template)
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
                Creator = Environment.WhitePages.FilteredAgentIdsByClassId(Class).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }
    }
}