#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using SymuEngine.Classes.Organization;
using SymuEngine.Repository.Networks;

#endregion

namespace SymuEngine.Results.Organization
{
    /// <summary>
    ///     Get the knowledge and Belief performance for the group
    /// </summary>
    public class OrganizationKnowledgeAndBelief
    {
        private readonly OrganizationModels _models;

        /// <summary>
        ///     Network of the simulation
        /// </summary>
        private readonly Network _network;

        public OrganizationKnowledgeAndBelief(Network network, OrganizationModels models)
        {
            _network = network;
            _models = models;
        }

        /// <summary>
        ///     List of knowledge performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Knowledges { get; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of belief performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Beliefs { get; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of learning performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Learning { get; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of forgetting performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Forgetting { get; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of Global Knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public List<KnowledgeAndBeliefStruct> KnowledgeObsolescence { get; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        /// Clear of results
        /// </summary>
        public void Clear()
        {
            Knowledges.Clear();
            Beliefs.Clear();
            Forgetting.Clear();
            Learning.Clear();
            KnowledgeObsolescence.Clear();
        }
        /// <summary>
        /// Handle the performance around knowledge and beliefs
        /// </summary>
        /// <param name="step"></param>
        public void HandlePerformance(ushort step)
        {
            if (!_models.FollowGroupKnowledge)
            {
                return;
            }

            HandleBelief(step);
            HandleKnowledge(step);
            HandleLearning(step);
            HandleForgetting(step);
            HandleKnowledgeObsolescence(step);
        }

        public void HandleLearning(ushort step)
        {
            var sum = _network.NetworkKnowledges.AgentsRepository.Values.Select(e => e.Learning).ToList();
            var learning = SetStructKnowledgeAndBeliefStruct(step, sum);
            Learning.Add(learning);
        }

        public void HandleForgetting(ushort step)
        {
            var sum = _network.NetworkKnowledges.AgentsRepository.Values.Select(e => e.Forgetting).ToList();
            var forgetting = SetStructKnowledgeAndBeliefStruct(step, sum);
            Forgetting.Add(forgetting);
        }

        public void HandleKnowledgeObsolescence(ushort step)
        {
            var sum = _network.NetworkKnowledges.AgentsRepository.Values.Select(e => e.Obsolescence).ToList();
            var obsolescence = SetStructKnowledgeAndBeliefStruct(step, sum);
            KnowledgeObsolescence.Add(obsolescence);
        }

        public void HandleKnowledge(ushort step)
        {
            var sumKnowledge = _network.NetworkKnowledges.AgentsRepository.Values
                .Select(expertise => expertise.GetKnowledgesSum()).ToList();
            var knowledge = SetStructKnowledgeAndBeliefStruct(step, sumKnowledge);
            Knowledges.Add(knowledge);
        }

        private static KnowledgeAndBeliefStruct SetStructKnowledgeAndBeliefStruct(ushort step, List<float> sumKnowledge)
        {
            float sum;
            float mean;
            float stdDev;
            switch (sumKnowledge.Count)
            {
                case 0:
                    sum = 0;
                    mean = 0;
                    stdDev = 0;
                    break;
                case 1:
                    sum = sumKnowledge[0];
                    mean = sumKnowledge[0];
                    stdDev = 0;
                    break;
                default:
                    sum = sumKnowledge.Sum();
                    mean = sumKnowledge.Average();
                    stdDev = (float) sumKnowledge.StandardDeviation();
                    break;
            }

            var knowledge = new KnowledgeAndBeliefStruct(sum, mean, stdDev, step);
            return knowledge;
        }

        public void HandleBelief(ushort step)
        {
            var sum = _network.NetworkBeliefs.AgentsRepository.Values.Select(beliefs => beliefs.GetBeliefsSum())
                .ToList();
            var belief = SetStructKnowledgeAndBeliefStruct(step, sum);
            Beliefs.Add(belief);
        }
    }
}