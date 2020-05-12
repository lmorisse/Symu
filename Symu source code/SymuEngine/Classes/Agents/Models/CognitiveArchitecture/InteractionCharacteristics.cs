#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Messaging.Messages;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveArchitecture
{
    /// <summary>
    ///     Interaction Characteristics from Construct Software
    ///     Interactions
    ///     Initiations (of interactions)
    ///     Receptions
    ///     Distinct message
    /// </summary>
    public class InteractionCharacteristics
    {
        public void CopyTo(InteractionCharacteristics interactionCharacteristics)
        {
            if (interactionCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(interactionCharacteristics));
            }

            interactionCharacteristics.LimitMessagesPerPeriod = LimitMessagesPerPeriod;
            interactionCharacteristics.MaximumMessagesPerPeriod = MaximumMessagesPerPeriod;
            interactionCharacteristics.LimitMessagesSentPerPeriod = LimitMessagesSentPerPeriod;
            interactionCharacteristics.MaximumMessagesSentPerPeriod = MaximumMessagesSentPerPeriod;
            interactionCharacteristics.LimitReceptionsPerPeriod = LimitReceptionsPerPeriod;
            interactionCharacteristics.MaximumReceptionsPerPeriod = MaximumReceptionsPerPeriod;
            interactionCharacteristics.PreferredCommunicationMediums = PreferredCommunicationMediums;
        }

        #region Interactions

        /// <summary>
        ///     specify if the total number of interactions in which an agent of this class can participate during each simulation
        ///     period is limited or not
        ///     If set to true, defines the MaximumIterationsPerPeriod parameter
        ///     If an agent has exceeded the MaximumMessagesPerPeriod, the next messages will be considered as missed (not
        ///     postponed)
        /// </summary>
        /// <remarks>It concerns all messages except system messages</remarks>
        public bool LimitMessagesPerPeriod { get; set; }

        /// <summary>
        ///     specify the total number of interactions in which an agent of this class can participate during each simulation
        ///     period.
        ///     The total number of interactions each period is the sum of all initiated and received interactions, so limiting
        ///     this number will either limit the number of initiations, receptions, or both.
        ///     An agent that cannot interact with any agent should have a maximum interaction value of zero
        /// </summary>
        /// <remarks>MaximumInteractionsPerPeriod in Construct Software</remarks>
        public byte MaximumMessagesPerPeriod { get; set; }

        public CommunicationMediums PreferredCommunicationMediums { get; set; }

        #endregion

        #region Initiations

        /// <summary>
        ///     specify if the number of messages in which an agent can initiate during each simulation period is limited or not
        ///     If set to true, defines the MaximumInitiationsPerPeriod parameter
        ///     If an agent has exceeded the MaximumInitiationsPerPeriod, the next messages will be considered as missed (not
        ///     postponed)
        /// </summary>
        /// <remarks>It concerns all messages except system messages</remarks>
        public bool LimitMessagesSentPerPeriod { get; set; }

        /// <summary>
        ///     specify the total number of interactions that an agent will initiate during each simulation period.
        ///     An agent that cannot initiate contact with any agent should have a maximum initiation value of zero.
        ///     Agents will never exceed more than their maximum initiation count.
        /// </summary>
        /// <remarks>MaximumInitiationsPerPeriod in Construct Software</remarks>
        public byte MaximumMessagesSentPerPeriod { get; set; }

        #endregion

        #region Receptions

        /// <summary>
        ///     specify if the number of messages in which an agent can received during each simulation period is limited or not
        ///     If set to true, defines the MaximumReceptionsPerPeriod parameter
        ///     If an agent has exceeded the MaximumReceptionsPerPeriod, the next messages will be considered as missed (not
        ///     postponed)
        /// </summary>
        /// <remarks>It concerns all messages except system messages</remarks>
        public bool LimitReceptionsPerPeriod { get; set; }

        /// <summary>
        ///     specify the total number of interactions that an agent can receive during each simulation period.
        ///     Agents will never exceed more than their maximum reception count,
        /// </summary>
        /// <remarks>MaximumInitiationsPerPeriod in Construct Software</remarks>
        public byte MaximumReceptionsPerPeriod { get; set; }

        #endregion
    }
}