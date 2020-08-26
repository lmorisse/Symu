#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Repository.Entity;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     Message content from Construct Software
    ///     Send & receive :
    ///     Knowledge
    ///     Beliefs
    ///     with transactive memory
    ///     Referral
    /// </summary>
    public class MessageContent
    {
        public void CopyTo(MessageContent messageContent)
        {
            if (messageContent is null)
            {
                throw new ArgumentNullException(nameof(messageContent));
            }

            #region Knowledge

            messageContent.CanSendKnowledge = CanSendKnowledge;
            messageContent.CanReceiveKnowledge = CanReceiveKnowledge;
            messageContent.MinimumKnowledgeToSendPerBit = MinimumKnowledgeToSendPerBit;
            messageContent.MinimumNumberOfBitsOfKnowledgeToSend = MinimumNumberOfBitsOfKnowledgeToSend;
            messageContent.MaximumNumberOfBitsOfKnowledgeToSend = MaximumNumberOfBitsOfKnowledgeToSend;

            #endregion

            #region Belief

            messageContent.CanSendBeliefs = CanSendBeliefs;
            messageContent.CanReceiveBeliefs = CanReceiveBeliefs;
            messageContent.MinimumBeliefToSendPerBit = MinimumBeliefToSendPerBit;
            messageContent.MinimumNumberOfBitsOfBeliefToSend = MinimumNumberOfBitsOfBeliefToSend;
            messageContent.MaximumNumberOfBitsOfBeliefToSend = MaximumNumberOfBitsOfBeliefToSend;

            #endregion
        }

        #region Knowledge

        public bool CanSendKnowledge { get; set; }
        public bool CanReceiveKnowledge { get; set; }

        private float _minimumKnowledgeToSendPerBit = 0.35F;

        /// <summary>
        ///     To send Knowledge, an agent must have enough knowledge per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <remarks>Default value = Min value of the level foundational</remarks>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent KnowledgeId[index] = 0.6 => he can answer to the question</example>
        public float MinimumKnowledgeToSendPerBit
        {
            get => _minimumKnowledgeToSendPerBit;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinimumKnowledgeToSendPerBit should be between 0 and 1");
                }

                _minimumKnowledgeToSendPerBit = value;
            }
        }

        private byte _minimumNumberOfBitsOfKnowledgeToSend = 1;

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfKnowledgeToSend
        {
            get => _minimumNumberOfBitsOfKnowledgeToSend;
            set
            {
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("MinimumNumberOfBitsOfKnowledgeToSend should be <= " +
                                                          Bits.MaxBits);
                }

                _minimumNumberOfBitsOfKnowledgeToSend = value;
            }
        }

        private byte _maximumNumberOfBitsOfKnowledgeToSend = 1;

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfKnowledgeToSend
        {
            get => _maximumNumberOfBitsOfKnowledgeToSend;
            set
            {
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("MaximumNumberOfBitsOfKnowledgeToSend should be <= " +
                                                          Bits.MaxBits);
                }

                _maximumNumberOfBitsOfKnowledgeToSend = value;
            }
        }

        #endregion

        #region Beliefs

        public bool CanSendBeliefs { get; set; }
        public bool CanReceiveBeliefs { get; set; }

        private float _minimumBeliefToSendPerBit = 0.35F;

        /// <summary>
        ///     To send beliefs, an agent must have enough beliefs per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent BeliefId[index] = 0.6 => he can answer to the question</example>
        /// <remarks>Default value = Min value of the level Foundational</remarks>
        public float MinimumBeliefToSendPerBit
        {
            get => _minimumBeliefToSendPerBit;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinimumBeliefToSendPerBit should be between 0 and 1");
                }

                _minimumBeliefToSendPerBit = value;
            }
        }

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        #endregion

        #region Referral

        #endregion
    }
}