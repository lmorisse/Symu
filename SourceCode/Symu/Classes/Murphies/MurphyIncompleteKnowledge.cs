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
using Symu.Common.Interfaces.Entity;
using Symu.Repository.Entity;

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
        public void CheckKnowledge(IId knowledgeId, TaskKnowledgeBits taskBitIndexes, KnowledgeModel knowledgeModel,
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
            var workerKnowledge = knowledgeModel.Expertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId);
            if (workerKnowledge == null)
            {
                return;
            }

            mandatoryCheck = knowledgeModel.Check(workerKnowledge, taskBitIndexes.GetMandatory(), out mandatoryIndex,
                ThresholdForReacting, step);
            requiredCheck = knowledgeModel.Check(workerKnowledge, taskBitIndexes.GetRequired(), out requiredIndex,
                ThresholdForReacting, step);
        }

        /// <summary>
        ///     Check Knowledge required against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit">KnowledgeBit index of the task that must be checked against worker Knowledge</param>
        /// <param name="knowledgeModel"></param>
        /// <param name="step"></param>
        /// <returns>True if the knowledgeBit is known enough</returns>
        public bool CheckKnowledge(IId knowledgeId, byte knowledgeBit, KnowledgeModel knowledgeModel, ushort step)
        {
            if (!IsAgentOn())
            {
                return false;
            }

            // workerKnowledge may don't have the knowledge at all
            return knowledgeModel.KnowsEnough(knowledgeId, knowledgeBit, ThresholdForReacting, step);
            //var workerKnowledge = expertise?.GetKnowledge(knowledngeId);
            //return workerKnowledge != null &&
            //       workerKnowledge.KnowsEnough(knowledgeBit, ThresholdForReacting,
            //           step);
        }
    }
}