#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;
using Symu.Common.Interfaces;
using Symu.Environment;

#endregion

namespace Symu.Results.Organization
{
    /// <summary>
    ///     Get the knowledge and Belief performance for the group
    /// </summary>
    public sealed class KnowledgeAndBeliefResults : Result
    {
        public KnowledgeAndBeliefResults(SymuEnvironment environment) : base(environment)
        {
        }

        /// <summary>
        ///     List of knowledge performance per step
        /// </summary>
        public List<StatisticalResultStruct> Knowledge { get; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of belief performance per step
        /// </summary>
        public List<StatisticalResultStruct> Beliefs { get; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of learning performance per step
        /// </summary>
        public List<StatisticalResultStruct> Learning { get; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of forgetting performance per step
        /// </summary>
        public List<StatisticalResultStruct> Forgetting { get; } = new List<StatisticalResultStruct>();

        /// <summary>
        ///     List of Global Knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public List<StatisticalResultStruct> KnowledgeObsolescence { get; } =
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
        public override void SetResults()
        {
            HandleBelief();
            HandleKnowledge();
            HandleLearning();
            HandleForgetting();
            HandleKnowledgeObsolescence();
        }

        public void HandleLearning()
        {
            var sum = Environment.WhitePages.AllCognitiveAgents()
                .Select(e => e.LearningModel.CumulativeLearning)
                .ToList();
            var potentialKnowledge = Environment.WhitePages.AllCognitiveAgents()
                .Sum(e => e.KnowledgeModel.GetKnowledgePotential());
            var learning = StatisticalResultStruct.SetStruct(Environment.Schedule.Step, sum, potentialKnowledge);
            Learning.Add(learning);
        }

        public void HandleForgetting()
        {
            var sum = Environment.WhitePages.AllCognitiveAgents()
                .Select(e => e.ForgettingModel.CumulativeForgetting).ToList();
            var sumKnowledge = Environment.WhitePages.AllCognitiveAgents()
                .Sum(e => e.KnowledgeModel.GetKnowledgeSum());
            var forgetting = StatisticalResultStruct.SetStruct(Environment.Schedule.Step, sum, sumKnowledge);
            Forgetting.Add(forgetting);
        }

        public void HandleKnowledgeObsolescence()
        {
            var sum = Environment.WhitePages.AllCognitiveAgents()
                .Select(e => e.KnowledgeModel.Obsolescence(Environment.Schedule.Step)).ToList();
            var potentialKnowledge = Environment.WhitePages.AllCognitiveAgents()
                .Sum(e => e.KnowledgeModel.GetKnowledgePotential());
            var obsolescence = StatisticalResultStruct.SetStruct(Environment.Schedule.Step, sum, potentialKnowledge);
            KnowledgeObsolescence.Add(obsolescence);
        }

        public void HandleKnowledge()
        {
            var sum = Environment.WhitePages.AllCognitiveAgents()
                .Select(e => e.KnowledgeModel.GetKnowledgeSum()).ToList();
            var potential = Environment.WhitePages.AllCognitiveAgents()
                .Sum(e => e.KnowledgeModel.GetKnowledgePotential());
            var knowledge = StatisticalResultStruct.SetStruct(Environment.Schedule.Step, sum, potential);
            Knowledge.Add(knowledge);
        }

        public void HandleBelief()
        {
            var sum = Environment.WhitePages.AllCognitiveAgents()
                .Select(e => e.BeliefsModel.GetBeliefsSum()).ToList();
            var potential = Environment.WhitePages.AllCognitiveAgents()
                .Sum(e => e.BeliefsModel.GetBeliefsPotential());
            var belief = StatisticalResultStruct.SetStruct(Environment.Schedule.Step, sum, potential);
            Beliefs.Add(belief);
        }

        public override void CopyTo(object clone)
        {
            if (!(clone is KnowledgeAndBeliefResults cloneKnowledgeAndBeliefResults))
            {
                return;
            }

            //cloneKnowledgeAndBeliefResults.Knowledge = new List<StatisticalResultStruct>();
            foreach (var result in Knowledge)
            {
                cloneKnowledgeAndBeliefResults.Knowledge.Add(result);
            }

            //cloneKnowledgeAndBeliefResults.Beliefs = new List<StatisticalResultStruct>();
            foreach (var result in Beliefs)
            {
                cloneKnowledgeAndBeliefResults.Beliefs.Add(result);
            }

            //cloneKnowledgeAndBeliefResults.Learning = new List<StatisticalResultStruct>();
            foreach (var result in Learning)
            {
                cloneKnowledgeAndBeliefResults.Learning.Add(result);
            }

            //cloneKnowledgeAndBeliefResults.Forgetting = new List<StatisticalResultStruct>();
            foreach (var result in Forgetting)
            {
                cloneKnowledgeAndBeliefResults.Forgetting.Add(result);
            }

            //cloneKnowledgeAndBeliefResults.KnowledgeObsolescence = new List<StatisticalResultStruct>();
            foreach (var result in KnowledgeObsolescence)
            {
                cloneKnowledgeAndBeliefResults.KnowledgeObsolescence.Add(result);
            }
        }

        public override IResult Clone()
        {
            var clone = new KnowledgeAndBeliefResults(Environment);
            CopyTo(clone);
            return clone;
        }
    }
}