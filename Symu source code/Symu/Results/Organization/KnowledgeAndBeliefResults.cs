#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using Symu.Common;
using Symu.Environment;

#endregion

namespace Symu.Results.Organization
{
    /// <summary>
    ///     Get the knowledge and Belief performance for the group
    /// </summary>
    public class KnowledgeAndBeliefResults
    {
        private readonly SymuEnvironment _environment;

        public KnowledgeAndBeliefResults(SymuEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        ///     If set to true, KnowledgeAndBeliefResults will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        public TimeStepType Frequency { get; set; } = TimeStepType.Monthly;

        /// <summary>
        ///     List of knowledge performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Knowledge { get; private set; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of belief performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Beliefs { get; private set; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of learning performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Learning { get; private set; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of forgetting performance per step
        /// </summary>
        public List<KnowledgeAndBeliefStruct> Forgetting { get; private set; } = new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     List of Global Knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public List<KnowledgeAndBeliefStruct> KnowledgeObsolescence { get; private set; } =
            new List<KnowledgeAndBeliefStruct>();

        /// <summary>
        ///     Initialize of results
        /// </summary>
        public void Clear()
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
        /// <param name="schedule"></param>
        public void SetResults(Schedule schedule)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException(nameof(schedule));
            }

            if (!On)
            {
                return;
            }

            bool handle;
            switch (Frequency)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    handle = true;
                    break;
                case TimeStepType.Weekly:
                    handle = schedule.IsEndOfWeek;
                    break;
                case TimeStepType.Monthly:
                    handle = schedule.IsEndOfMonth;
                    break;
                case TimeStepType.Yearly:
                    handle = schedule.IsEndOfYear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!handle)
            {
                return;
            }
            HandleBelief(schedule.Step);
            HandleKnowledge(schedule.Step);
            HandleLearning(schedule.Step);
            HandleForgetting(schedule.Step);
            HandleKnowledgeObsolescence(schedule.Step);
        }

        public void HandleLearning(ushort step)
        {
            var sum = _environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values.Select(e => e.Learning)
                .ToList();
            var learning = SetStructKnowledgeAndBeliefStruct(step, sum);
            Learning.Add(learning);
        }

        public void HandleForgetting(ushort step)
        {
            var sum = _environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(e => e.Forgetting).ToList();
            var forgetting = SetStructKnowledgeAndBeliefStruct(step, sum);
            Forgetting.Add(forgetting);
        }

        public void HandleKnowledgeObsolescence(ushort step)
        {
            var sum = _environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(e => e.Obsolescence).ToList();
            var obsolescence = SetStructKnowledgeAndBeliefStruct(step, sum);
            KnowledgeObsolescence.Add(obsolescence);
        }

        public void HandleKnowledge(ushort step)
        {
            var sumKnowledge = _environment.WhitePages.Network.NetworkKnowledges.AgentsRepository.Values
                .Select(expertise => expertise.GetKnowledgesSum()).ToList();
            var knowledge = SetStructKnowledgeAndBeliefStruct(step, sumKnowledge);
            Knowledge.Add(knowledge);
        }

        private static KnowledgeAndBeliefStruct SetStructKnowledgeAndBeliefStruct(ushort step,
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

            var knowledge = new KnowledgeAndBeliefStruct(sum, mean, stdDev, step);
            return knowledge;
        }

        public void HandleBelief(ushort step)
        {
            var sum = _environment.WhitePages.Network.NetworkBeliefs.AgentsRepository.Values
                .Select(beliefs => beliefs.GetBeliefsSum())
                .ToList();
            var belief = SetStructKnowledgeAndBeliefStruct(step, sum);
            Beliefs.Add(belief);
        }

        public void CopyTo(KnowledgeAndBeliefResults cloneKnowledgeAndBeliefResults)
        {
            if (cloneKnowledgeAndBeliefResults == null)
            {
                throw new ArgumentNullException(nameof(cloneKnowledgeAndBeliefResults));
            }

            cloneKnowledgeAndBeliefResults.Knowledge = new List<KnowledgeAndBeliefStruct>();
            foreach (var result in Knowledge)
            {
                cloneKnowledgeAndBeliefResults.Knowledge.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Beliefs = new List<KnowledgeAndBeliefStruct>();
            foreach (var result in Beliefs)
            {
                cloneKnowledgeAndBeliefResults.Beliefs.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Learning = new List<KnowledgeAndBeliefStruct>();
            foreach (var result in Learning)
            {
                cloneKnowledgeAndBeliefResults.Learning.Add(result);
            }

            cloneKnowledgeAndBeliefResults.Forgetting = new List<KnowledgeAndBeliefStruct>();
            foreach (var result in Forgetting)
            {
                cloneKnowledgeAndBeliefResults.Forgetting.Add(result);
            }

            cloneKnowledgeAndBeliefResults.KnowledgeObsolescence = new List<KnowledgeAndBeliefStruct>();
            foreach (var result in KnowledgeObsolescence)
            {
                cloneKnowledgeAndBeliefResults.KnowledgeObsolescence.Add(result);
            }
        }
    }
}