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
using Symu.Repository.Networks.Beliefs;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace Symu.Classes.Murphies
{
    /// <summary>
    ///     The belief of the worker hav an impact on the way he work on a specific Task
    ///     If so, task may be blocked or incorrectly prioritize
    /// </summary>
    public class MurphyIncompleteBelief : MurphyTask
    {
        /// <summary>
        ///     Due to belief, agent ask help to other agents via different mediums
        ///     Communication mediums allowed for belief
        /// </summary>
        public CommunicationMediums CommunicationMediums { get; set; }

        private float _rateOfIncorrectGuess = 0.3F;

        /// <summary>
        ///     Rate of incorrect task
        ///     If worker doesn't have enough knowledge for the task, the worker can guess and complete the task
        ///     The task may be correct or incorrect
        ///     [0 - 1]
        /// </summary>
        /// <example>if rate = 0.3 (default), 3 tasks out of 10 will be incorrects</example>
        public float RateOfIncorrectGuess
        {
            get => _rateOfIncorrectGuess;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("RateOfIncorrectGuess should be between [0;1]");
                }

                _rateOfIncorrectGuess = value;
            }
        }

        private float _rateOfAnswers = 0.5F;

        /// <summary>
        ///     Rate of answers
        ///     If worker is blocked by the task, he may ask for help, but not everyone answer
        ///     [0 - 1]
        /// </summary>
        /// <example>if rate = 0.1 , 1 teammate out of 10 will answer</example>
        public float RateOfAnswers
        {
            get => _rateOfAnswers;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("RateOfAnswers should be between [0;1]");
                }

                _rateOfAnswers = value;
            }
        }

        /// <summary>
        ///     Delay to answer in days
        ///     Those who answer may take some time to reply
        /// </summary>
        /// <example>if 1 (default), teammate will answer the next day</example>
        public byte ResponseTime { get; set; } = 1;

        /// <summary>
        ///     The agent may try a certain amount of times before having an answer.
        ///     LimitNumberOfTries is used to limit this number of tries.
        ///     Once the limit reach, the agent will guess the answer
        /// </summary>
        /// <example>if -1 (default), there is no limit</example>
        /// <example>if =3, agent will try 3 times before guessing the answer</example>
        public sbyte LimitNumberOfTries { get; set; } = -1;

        /// <summary>
        ///     Impact On Time Spent Ratio
        ///     Agent will take longer to complete the task
        /// </summary>
        /// <example>if 1 (default), teammate will answer the next day</example>
        public byte ImpactOnTimeSpentRatio { get; set; } = 1;
        private float _beliefThreshHoldForReacting = 0.1F;

        /// <summary>
        ///     To react, an agent must have enough belief
        ///     BeliefThreshHoldForReacting should be inferior to RiskAversionThreshold
        ///     [0 - 1]
        /// </summary>
        /// <example>if BeliefThreshHoldForReacting = 0.05 and agent BeliefId[index] = 0.6 => he won't react</example>
        public float BeliefThreshHoldForReacting
        {
            get => _beliefThreshHoldForReacting;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("BeliefThreshHoldForReacting should be between 0 and 1");
                }

                _beliefThreshHoldForReacting = value;
            }
        }

        /// <summary>
        ///     Check belief required by a task against the worker expertise
        /// </summary>
        /// <param name="belief"></param>
        /// <param name="taskBitIndexes">KnowledgeBit indexes of the task that must be checked against agent's beliefs</param>
        /// <param name="agentBeliefs"></param>
        /// <param name="mandatoryCheck">The normalized score of the agent belief [-1; 1] at the first mandatoryIndex that is above beliefThreshHoldForReacting</param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <returns>0 if agent has no beliefs</returns>
        public void CheckBelief(Belief belief, TaskKnowledgeBits taskBitIndexes, AgentBeliefs agentBeliefs,  ref float mandatoryCheck,
            ref float requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }
            if (belief is null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            // model is off
            if (!IsAgentOn())
            {
                mandatoryCheck = 0;
                requiredCheck = 0;
                return;
            }
            // agent may don't have the belief at all
            var agentBelief = agentBeliefs?.GetBelief(belief.Id);
            if (agentBelief == null)
            {
                mandatoryCheck = 0;
                requiredCheck = 0;
                return;
            }

            mandatoryCheck = agentBelief.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex, belief,
                BeliefThreshHoldForReacting);
            requiredCheck = agentBelief.Check(taskBitIndexes.GetRequired(), out requiredIndex, belief,
                BeliefThreshHoldForReacting);
        }

        /// <summary>
        ///     an agent ask for help, but he can choose different channels like email, phone, ...
        /// </summary>
        /// <returns></returns>
        public CommunicationMediums AskOnWhichChannel(CommunicationMediums preferredMediums)
        {
            var intersectMediums = preferredMediums & CommunicationMediums;
            return CommunicationMediumsModel.AskOnWhichChannel(intersectMediums);
        }

        /// <summary>
        ///     Check if agent should guess or try to recover the blocker
        /// </summary>
        /// <param name="numberOfTries"></param>
        /// <returns>true if agent should guess, false if agent should try to recover the blocker</returns>
        public bool ShouldGuess(byte numberOfTries)
        {
            return LimitNumberOfTries > -1 && numberOfTries > LimitNumberOfTries;
        }

        /// <summary>
        ///     Return the new Type of missing information of the task
        /// </summary>
        /// <returns>
        ///     The type of missing information
        ///     None if there is no missing information
        /// </returns>
        public ImpactLevel NextGuess()
        {
            if (!On)
            {
                return ImpactLevel.None;
            }

            if (!Bernoulli.Sample(RateOfIncorrectGuess))
            {
                return ImpactLevel.None;
            }

            switch (DiscreteUniform.SampleToByte(3))
            {
                //case 0:
                default:
                    return ImpactLevel.Micro;
                case 1:
                    return ImpactLevel.Minor;
                case 2:
                    return ImpactLevel.Major;
                case 3:
                    return ImpactLevel.Critical;
            }
        }

        /// <summary>
        ///     Return the impact on time spent
        ///     Because agent lack of belief on the task, agent will take more time to complete the task
        /// </summary>
        /// <returns>0 if model is off</returns>
        /// <returns>random value if model is on based on ImpactOnTimeSpentRatio</returns>
        public float NextImpactOnTimeSpent()
        {
            //TODO with ImpactOnTimeSpentRatio = 0 => impact may be < 0
            //belief may have a positive or negative on the time spent on a task
            //less time if agent is not motivated
            //too much time if agent is motivated
            //For the moment, only added time spent is considered
            if (!On)
            {
                return 0;
            }

            return Normal.Sample(ImpactOnTimeSpentRatio, 0.1F);
        }

        /// <summary>
        ///     does the teammate respond to the request for help?
        /// </summary>
        /// <returns>
        ///     -1 : no reply
        ///     0 : reply without delay
        ///     > 0 :reply with a delay
        /// </returns>
        public sbyte DelayToReplyToHelp()
        {
            sbyte reply = -1;
            // if Rate = 1, blocked if random == 0
            if (Bernoulli.Sample(RateOfAnswers))
                // Teammate want to answer with a responseTime
            {
                reply = (sbyte) DiscreteUniform.SampleToByte(ResponseTime);
            }

            return reply;
        }
    }
}