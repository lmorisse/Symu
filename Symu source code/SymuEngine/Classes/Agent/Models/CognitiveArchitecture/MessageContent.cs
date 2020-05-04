#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Classes.Agent.Models.Templates.Communication;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Classes.ProbabilityDistributions;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     Message content from Construct Software
    ///     Send & receive :
    ///     Knowledges
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

        /// <summary>
        ///     To send Knowledge, an agent must have enough knowledge per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <remarks>Default value = Min value of the level foundational</remarks>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent KnowledgeId[index] = 0.6 => he can answer to the question</example>
        public float MinimumKnowledgeToSendPerBit { get; set; } = 0.35F;

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfKnowledgeToSend { get; set; } = 1;

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfKnowledgeToSend { get; set; } = 1;

        /// <summary>
        ///     stochastic Filtering agentKnowledge based on MinimumBitsOfKnowledgeToSend/MaximumBitsOfKnowledgeToSend
        ///     Work with non binary KnowledgeBits
        /// </summary>
        /// <param name="agentKnowledge">Full agentKnowledge</param>
        /// <param name="knowledgeBit"></param>
        /// <param name="medium"></param>
        /// <param name="knowledgeIndexToSend"></param>
        /// <returns>With binary KnowledgeBits it will return a float of 0</returns>
        /// <example>KnowledgeBits[0,1,0.6] and MinimumKnowledgeToSend = 0.8 => KnowledgeBits[0,1,0]</example>
        public Bits GetFilteredKnowledgeToSend(AgentKnowledge agentKnowledge, byte knowledgeBit,
            CommunicationTemplate medium, out byte[] knowledgeIndexToSend)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            knowledgeIndexToSend = null;
            // Model On/Off
            if (!CanSendKnowledge)
            {
                return null;
            }

            // Filtering via the good channel
            var minBits = Math.Max(MinimumNumberOfBitsOfKnowledgeToSend,
                medium.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend);
            var maxBits = Math.Min(MaximumNumberOfBitsOfKnowledgeToSend,
                medium.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend);
            var minKnowledge = Math.Max(MinimumKnowledgeToSendPerBit,
                medium.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit);
            // Random knowledgeBits to send
            var lengthToSend = DiscreteUniform.SampleToByte(Math.Min(minBits, maxBits), Math.Max(minBits, maxBits));
            if (lengthToSend == 0)
            {
                return null;
            }

            knowledgeIndexToSend = DiscreteUniform.SamplesToByte(lengthToSend, agentKnowledge.Length - 1);
            // Force the first index of the knowledgeIndex To send to be the knowledgeBit asked by an agent
            knowledgeIndexToSend[0] = knowledgeBit;
            var knowledgeBitsToSend = Bits.Initialize(agentKnowledge.Length, 0F);
            for (byte i = 0; i < knowledgeIndexToSend.Length; i++)
            {
                var index = knowledgeIndexToSend[i];
                if (agentKnowledge.GetKnowledgeBit(index) >= minKnowledge)
                {
                    knowledgeBitsToSend[index] = agentKnowledge.GetKnowledgeBit(index);
                }
            }

            var bitsToSend = new Bits(knowledgeBitsToSend, 0);
            // Check Length of message
            // We don't find always what we were looking for
            return Math.Abs(bitsToSend.GetSum()) < Tolerance ? null : bitsToSend;
        }

        #endregion

        #region Beliefs

        public bool CanSendBeliefs { get; set; }
        public bool CanReceiveBeliefs { get; set; }

        /// <summary>
        ///     To send Knowledge, an agent must have enough knowledge per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent KnowledgeId[index] = 0.6 => he can answer to the question</example>
        /// <remarks>Default value = Min value of the level Foundational</remarks>
        public float MinimumBeliefToSendPerBit { get; set; } = 0.35F;

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        /// <summary>
        ///     stochastic Filtering agentKnowledge based on MinimumBitsOfKnowledgeToSend/MaximumBitsOfKnowledgeToSend
        ///     Work with non binary KnowledgeBits
        /// </summary>
        /// <param name="agentBelief">Full agentKnowledge</param>
        /// <param name="beliefBit"></param>
        /// <param name="medium"></param>
        /// <returns>With binary KnowledgeBits it will return a float of 0</returns>
        /// <example>KnowledgeBits[0,1,0.6] and MinimumKnowledgeToSend = 0.8 => KnowledgeBits[0,1,0]</example>
        public Bits GetFilteredBeliefToSend(AgentBelief agentBelief, byte beliefBit, CommunicationTemplate medium)
        {
            if (agentBelief is null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            // Model On/Off
            if (!CanSendBeliefs)
            {
                return null;
            }

            // Filtering via the good channel
            var minBits = Math.Max(MinimumNumberOfBitsOfBeliefToSend,
                medium.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend);
            var maxBits = Math.Min(MaximumNumberOfBitsOfBeliefToSend,
                medium.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend);
            var minKnowledge = Math.Max(MinimumBeliefToSendPerBit,
                medium.Cognitive.MessageContent.MinimumBeliefToSendPerBit);
            // Random knowledgeBits to send
            var lengthToSend = DiscreteUniform.SampleToByte(minBits, maxBits);
            if (lengthToSend == 0)
            {
                return null;
            }

            var beliefIndexToSend = DiscreteUniform.SamplesToByte(lengthToSend, agentBelief.Length - 1);
            // Force the first index of the knowledgeIndex To send to be the knowledgeBit asked by an agent
            beliefIndexToSend[0] = beliefBit;
            var beliefBitsToSend = agentBelief.CloneWrittenBeliefBits(MinimumBeliefToSendPerBit);
            // knowledgeBitsToSend full of 0 except for the random indexes knowledgeIndexToSend
            for (byte i = 0; i < beliefBitsToSend.Length; i++)
            {
                if (!beliefIndexToSend.Contains(i) || Math.Abs(beliefBitsToSend.GetBit(i)) < minKnowledge)
                {
                    beliefBitsToSend.SetBit(i, 0);
                }
            }

            // Check Length of message
            // We don't find always what we were looking for
            return Math.Abs(beliefBitsToSend.GetSum()) < Tolerance ? null : beliefBitsToSend;
        }

        #endregion

        #region Referral

        #endregion
    }
}