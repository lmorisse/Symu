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
            var sum = _network.NetworkKnowledges.AgentsRepository.Values
                .Select(expertise => expertise.GetKnowledgesSum()).ToList();
            KnowledgeAndBeliefStruct knowledge;
            switch (sum.Count)
            {
                case 0:
                    knowledge = new KnowledgeAndBeliefStruct(0, 0, 0, step);
                    break;
                case 1:
                    knowledge = new KnowledgeAndBeliefStruct(sum[0], sum[0], 0, step);
                    break;
                default:
                    knowledge = new KnowledgeAndBeliefStruct(sum.Sum(), sum.Average(), (float) sum.StandardDeviation(),
                        step);
                    break;
            }

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
            switch (sum.Count)
            {
                case 0:
                    belief = new KnowledgeAndBeliefStruct(0, 0, 0, step);
                    break;
                case 1:
                    belief = new KnowledgeAndBeliefStruct(sum[0], sum[0], 0, step);
                    break;
                default:
                    belief = new KnowledgeAndBeliefStruct(sum.Sum(), sum.Average(), (float) sum.StandardDeviation(),
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