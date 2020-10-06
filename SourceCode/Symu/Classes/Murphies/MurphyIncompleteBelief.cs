#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Task;
using Symu.DNA.Edges;
using Symu.DNA.GraphNetworks.TwoModesNetworks;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using ActorBelief = Symu.Repository.Edges.ActorBelief;

#endregion

namespace Symu.Classes.Murphies
{
    /// <summary>
    ///     The belief of the worker hav an impact on the way he work on a specific Task
    ///     If so, task may be blocked or incorrectly prioritize
    /// </summary>
    public class MurphyIncompleteBelief : MurphyIncomplete
    {
        /// <summary>
        ///     Check belief required by a task against the worker expertise
        /// </summary>
        /// <param name="belief"></param>
        /// <param name="taskBitIndexes">KnowledgeBit indexes of the task that must be checked against agent's beliefs</param>
        /// <param name="actorBelief"></param>
        /// <param name="mandatoryCheck">
        ///     The normalized score of the agent belief [-1; 1] at the first mandatoryIndex that is above
        ///     beliefThreshHoldForReacting
        /// </param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <returns>0 if agent has no beliefs</returns>
        public void CheckBelief(Belief belief, TaskKnowledgeBits taskBitIndexes, ActorBelief actorBelief,
            ref float mandatoryCheck,
            ref float requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            if (belief is null)
            {
                throw new ArgumentNullException(nameof(belief));
            }

            // model is off
            if (!IsAgentOn())
            {
                mandatoryCheck = 0;
                requiredCheck = 0;
                return;
            }

            // agent may don't have the belief at all
            //var actorBelief = actorBeliefs?.GetActorBelief<ActorBelief>(belief.EntityId);
            if (actorBelief == null)
            {
                mandatoryCheck = 0;
                requiredCheck = 0;
                return;
            }

            mandatoryCheck = actorBelief.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex, belief,
                ThresholdForReacting, true);
            requiredCheck = actorBelief.Check(taskBitIndexes.GetRequired(), out requiredIndex, belief,
                ThresholdForReacting, true);
        }

        public static void CheckRiskAversion(Belief belief, TaskKnowledgeBits taskBitIndexes, ActorBelief actorBelief,
            ref float mandatoryCheck, ref byte mandatoryIndex, float threshold)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            if (belief is null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            // agent may don't have the belief at all
            //var agentBelief = actorBeliefs?.GetActorBelief<ActorBelief>(belief.EntityId);
            if (actorBelief == null)
            {
                mandatoryCheck = 0;
                return;
            }

            mandatoryCheck = actorBelief.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex, belief,
                threshold, false);
        }
    }
}