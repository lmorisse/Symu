﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Task;
using SymuEngine.Common;
using SymuEngine.Messaging.Message;
using SymuTools.Classes.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Murphies
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