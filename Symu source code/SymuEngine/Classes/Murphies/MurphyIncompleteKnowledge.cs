#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Messaging.Messages;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Murphies
{
    /// <summary>
    ///     Tasks on which worker require more knowledges than the worker have
    ///     If so, task may be blocked or complete incorrectly
    /// </summary>
    public class MurphyIncompleteKnowledge : MurphyTask
    {
        /// <summary>
        ///     To do the task, an agent must have enough knowledge
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForDoing = 0.05 and agent KnowledgeId[index] = 0.6 => he can do to the question</example>
        public float KnowledgeThreshHoldForDoing { get; set; } = 0.1F;

        /// <summary>
        ///     Rate of incorrect task
        ///     If worker doesn't have enough knowledge for the task, the worker can guess and complete the task
        ///     The task may be correct or incorrect
        ///     [0 - 1]
        /// </summary>
        /// <example>if rate = 0.3 (default), 3 tasks out of 10 will be incorrects</example>
        public float RateOfIncorrectGuess { get; set; } = 0.3F;

        /// <summary>
        ///     Rate of answers
        ///     If worker is blocked by the task, he may ask for help, but not everyone answer
        ///     [0 - 1]
        /// </summary>
        /// <example>if rate = 0.1 , 1 teammate out of 10 will answer</example>
        public float RateOfAnswers { get; set; } = 0.5F;

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