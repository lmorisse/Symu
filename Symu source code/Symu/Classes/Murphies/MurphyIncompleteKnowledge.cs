#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Murphies
{
    /// <summary>
    ///     Tasks on which agent require more knowledges than the agent have
    ///     If so, task may be blocked or complete incorrectly
    /// </summary>
    public class MurphyIncompleteKnowledge : MurphyIncomplete
    {

        /// <summary>
        ///     Check Knowledge required by a task against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="taskBitIndexes">KnowledgeBits indexes of the task that must be checked against worker Knowledge</param>
        /// <param name="expertise"></param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <param name="step"></param>
        public void CheckKnowledge(ushort knowledgeId, TaskKnowledgeBits taskBitIndexes, AgentExpertise expertise, ref bool mandatoryCheck,
            ref bool requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex, ushort step)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }
            // model is off
            if (!IsAgentOn())
            {
                mandatoryCheck = true;
                requiredCheck = true;
                return;
            }

            // agent may don't have the knowledge at all
            var workerKnowledge = expertise?.GetKnowledge(knowledgeId);
            if (workerKnowledge == null)
            {
                return;
            }

            mandatoryCheck = workerKnowledge.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex,
                ThresholdForReacting, step);
            requiredCheck = workerKnowledge.Check(taskBitIndexes.GetRequired(), out requiredIndex,
                ThresholdForReacting, step);
        }

        /// <summary>
        ///     Check Knowledge required against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit">KnowledgeBit index of the task that must be checked against worker Knowledge</param>
        /// <param name="expertise"></param>
        /// <param name="step"></param>
        /// <returns>True if the knowledgeBit is known enough</returns>
        public bool CheckKnowledge(ushort knowledgeId, byte knowledgeBit, AgentExpertise expertise, ushort step)
        {
            if (!IsAgentOn())
            {
                return false;
            }

            // workerKnowledge may don't have the knowledge at all
            var workerKnowledge = expertise?.GetKnowledge(knowledgeId);
            return workerKnowledge != null &&
                   workerKnowledge.KnowsEnough(knowledgeBit, ThresholdForReacting,
                       step);
        }


    }
}