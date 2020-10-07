#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Classes.Task;
using Symu.Common.Interfaces;

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
        /// <param name="knowledgeModel"></param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <param name="step"></param>
        public void CheckKnowledge(IAgentId knowledgeId, TaskKnowledgeBits taskBitIndexes,
            KnowledgeModel knowledgeModel,
            ref bool mandatoryCheck,
            ref bool requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex, ushort step)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            if (knowledgeModel == null)
            {
                throw new ArgumentNullException(nameof(knowledgeModel));
            }

            // model is off
            if (!IsAgentOn())
            {
                mandatoryCheck = true;
                requiredCheck = true;
                return;
            }

            // agent may don't have the knowledge at all
            var actorKnowledge = knowledgeModel.GetActorKnowledge(knowledgeId);
            if (actorKnowledge == null)
            {
                return;
            }

            mandatoryCheck = knowledgeModel.Check(actorKnowledge, taskBitIndexes.GetMandatory(), out mandatoryIndex,
                ThresholdForReacting, step);
            requiredCheck = knowledgeModel.Check(actorKnowledge, taskBitIndexes.GetRequired(), out requiredIndex,
                ThresholdForReacting, step);
        }

        /// <summary>
        ///     Check Knowledge required against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit">KnowledgeBit index of the task that must be checked against worker Knowledge</param>
        /// <param name="knowledgeModel"></param>
        /// <param name="step"></param>
        /// <returns>False if the agent is On and if the knowledgeBit is not known enough</returns>
        /// <returns>True if the agent is not On or the knowledgeBit is known enough</returns>
        public bool CheckKnowledge(IAgentId knowledgeId, byte knowledgeBit, KnowledgeModel knowledgeModel, ushort step)
        {
            if (knowledgeModel == null)
            {
                throw new ArgumentNullException(nameof(knowledgeModel));
            }

            return !IsAgentOn() || knowledgeModel.KnowsEnough(knowledgeId, knowledgeBit, ThresholdForReacting, step);
        }
    }
}