#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Symu.Environment;

#endregion

namespace Symu.Results.Organization
{
    /// <summary>
    ///     Get the knowledge and Belief performance for the group
    /// </summary>
    public sealed class KnowledgeAndBeliefResults : SymuResults
    {
        public KnowledgeAndBeliefResults(SymuEnvironment environment) : base(environment)
        {
        }

        /// <summary>
        ///     List of knowledge performance per step
        /// </summary>
        public List<StatisticalResultStruct> Knowledge { get; private set; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of belief performance per step
        /// </summary>
        public List<StatisticalResultStruct> Beliefs { get; private set; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of learning performance per step
        /// </summary>
        public List<StatisticalResultStruct> Learning { get; private set; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of forgetting performance per step
        /// </summary>
        public List<StatisticalResultStruct> Forgetting { get; private set; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of Global Knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public List<StatisticalResultStruct> KnowledgeObsolescence { get; private set; } =
            new List<StatisticalResultStruct>();

        /// <summary>
        ///     Initialize of results
        /// </summary>
        public override void Clear()
        {
            Knowledge.Clear();
            Beliefs.Clear();
            Forgetting.Clear();
            Learning.Clear();
            KnowledgeObsolescence.Clear();
        }

        /// <summary>
        ///     Handle the performance around knowledge and beliefs
        /// </summary>
        protected override void HandleResults()
        {
            HandleBelief();
            HandleKnowledge();
            HandleLearning();
            HandleForgetting();
            HandleKnowledgeObsolescence();
        }

        public void HandleLearning()
        {
            var sum = Environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values.Select(e => e.Learning)
                .ToList();
            var learning = SetStructKnowledgeAndBeliefStruct(Environment.Schedule.Step, sum);
            Learning.Add(learning);
        }

        public void HandleForgetting()
        {
            var sum = Environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(e => e.Forgetting).ToList();
            var forgetting = SetStructKnowledgeAndBeliefStruct(Environment.Schedule.Step, sum);
            Forgetting.Add(forgetting);
        }

        public void HandleKnowledgeObsolescence()
        {
            var sum = Environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(e => e.Obsolescence).ToList();
            var obsolescence = SetStructKnowledgeAndBeliefStruct(Environment.Schedule.Step, sum);
            KnowledgeObsolescence.Add(obsolescence);
        }

        public void HandleKnowledge()
        {
            var sumKnowledge = Environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(expertise => expertise.GetKnowledgesSum()).ToList();
            var knowledge = SetStructKnowledgeAndBeliefStruct(Environment.Schedule.Step, sumKnowledge);
            Knowledge.Add(knowledge);
        }

        private static StatisticalResultStruct SetStructKnowledgeAndBeliefStruct(ushort step,
            IReadOnlyList<float> sumKnowledge)
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

            var knowledge = new StatisticalResultStruct(sum, mean, stdDev, step);
            return knowledge;
        }

        public void HandleBelief()
        {
            var sum = Environment.WhitePages.Network.NetworkBeliefs.AgentsRepository.Values
                .Select(beliefs => beliefs.GetBeliefsSum())
                .ToList();
            var belief = SetStructKnowledgeAndBeliefStruct(Environment.Schedule.Step, sum);
            Beliefs.Add(belief);
        }

        public override void CopyTo(object clone)
        {
            if (!(clone is KnowledgeAndBeliefResults cloneKnowledgeAndBeliefResults))
            {
                return;
            }

            cloneKnowledgeAndBeliefResults.Knowledge = new List<StatisticalResultStruct>();
            foreach (var result in Knowledge)
            {
                cloneKnowledgeAndBeliefResults.Knowledge.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Beliefs = new List<StatisticalResultStruct>();
            foreach (var result in Beliefs)
            {
                cloneKnowledgeAndBeliefResults.Beliefs.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Learning = new List<StatisticalResultStruct>();
            foreach (var result in Learning)
            {
                cloneKnowledgeAndBeliefResults.Learning.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Forgetting = new List<StatisticalResultStruct>();
            foreach (var result in Forgetting)
            {
                cloneKnowledgeAndBeliefResults.Forgetting.Add(result);
            }

            cloneKnowledgeAndBeliefResults.KnowledgeObsolescence = new List<StatisticalResultStruct>();
            foreach (var result in KnowledgeObsolescence)
            {
                cloneKnowledgeAndBeliefResults.KnowledgeObsolescence.Add(result);
            }
        }

        public override SymuResults Clone()
        {
            var clone = new KnowledgeAndBeliefResults(Environment);
            CopyTo(clone);
            return clone;
        }
    }
}