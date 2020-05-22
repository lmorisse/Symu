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
    public class MurphyIncompleteKnowledge : MurphyTask
    {
        private float _knowledgeThresholdForDoing = 0.1F;

        /// <summary>
        ///     To do the task, an agent must have enough knowledge.
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForReacting = 0.05 and agent KnowledgeId[index] = 0.6 => he can do to the question</example>
        public float KnowledgeThresholdForDoing
        {
            get => _knowledgeThresholdForDoing;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("KnowledgeThresholdForDoing should be between [0;1]");
                }

                _knowledgeThresholdForDoing = value;
            }
        }

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
        ///     Delay to switch from asking internally help to searching externally an answer
        /// </summary>
        /// <example>if 2 steps (default), worker will wait 2 days its co workers' answer before searching outside the org</example>
        public byte DelayBeforeSearchingExternally { get; set; } = 3;

        /// <summary>
        ///     The agent may try a certain amount of times before having an answer.
        ///     LimitNumberOfTries is used to limit this number of tries.
        ///     Once the limit reach, the agent will guess the answer
        /// </summary>
        /// <example>if -1 (default), there is no limit</example>
        /// <example>if =3, agent will try 3 times before guessing the answer</example>
        public sbyte LimitNumberOfTries { get; set; } = -1;

        /// <summary>
        ///     Due to lack of knowledge, agent ask help to other agents via different mediums
        ///     Communication mediums allowed for lack of knowledge
        /// </summary>
        public CommunicationMediums CommunicationMediums { get; set; }

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
                return;
            }

            // agent may don't have the knowledge at all
            var workerKnowledge = expertise?.GetKnowledge(knowledgeId);
            if (workerKnowledge == null)
            {
                return;
            }

            mandatoryCheck = workerKnowledge.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex,
                KnowledgeThresholdForDoing, step);
            requiredCheck = workerKnowledge.Check(taskBitIndexes.GetRequired(), out requiredIndex,
                KnowledgeThresholdForDoing, step);
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
                   workerKnowledge.KnowsEnough(knowledgeBit, KnowledgeThresholdForDoing,
                       step);
        }

        /// <summary>
        ///     Check if agent should guess or try to recover the blocker
        /// </summary>
        /// <param name="numberOfTries"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Check if worker ask internally some help or search externally an answer
        /// </summary>
        /// <param name="step">Actual step</param>
        /// <param name="initialStep">Initial step of the blocker</param>
        /// <returns>true if ask internally, false if ask externally</returns>
        public bool AskInternally(ushort step, ushort initialStep)
        {
            return step - initialStep < DelayBeforeSearchingExternally;
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
    }
}