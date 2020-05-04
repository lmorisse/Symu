#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
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

        public void HandleKnowledge(ushort step)
        {
            var sumKnowledge = _network.NetworkKnowledges.AgentsRepository.Values
                .Select(expertise => expertise.GetKnowledgesSum()).ToList();
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

            var learning = _network.NetworkKnowledges.AgentsRepository.Values.Sum(e => e.Learning);
            var forgetting = _network.NetworkKnowledges.AgentsRepository.Values.Sum(e => e.Forgetting);
            var obsolescence = _network.NetworkKnowledges.AgentsRepository.Values.Sum(e => e.Obsolescence);

            var knowledge = new KnowledgeAndBeliefStruct(sum, mean, stdDev, learning, forgetting, obsolescence, step);
            Knowledges.Add(knowledge);
        }

        public void Clear()
        {
            Knowledges.Clear();
            Beliefs.Clear();
        }

        public void HandleBelief(ushort step)
        {
            var sum = _network.NetworkBeliefs.AgentsRepository.Values.Select(beliefs => beliefs.GetBeliefsSum())
                .ToList();
            KnowledgeAndBeliefStruct belief;
            //TODO Gain/loss/obsolescence of the beliefs
            switch (sum.Count)
            {
                case 0:
                    belief = new KnowledgeAndBeliefStruct(0, 0, 0, 0, 0, 0, step);
                    break;
                case 1:
                    belief = new KnowledgeAndBeliefStruct(sum[0], sum[0], 0, 0, 0, 0, step);
                    break;
                default:
                    belief = new KnowledgeAndBeliefStruct(sum.Sum(), sum.Average(), (float) sum.StandardDeviation(), 0,
                        0, 0,
                        step);
                    break;
            }

            Beliefs.Add(belief);
        }

        public void HandlePerformance(ushort step)
        {
            if (!_models.FollowGroupKnowledge)
            {
                return;
            }

            HandleBelief(step);
            HandleKnowledge(step);
        }
    }
}